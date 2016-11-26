using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebExtensionPack
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(Vsix.Id)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class VSPackage : Package
    {
        protected override void Initialize()
        {
            Logger.Initialize(this, Vsix.Name);
            ResetExtensions.Initialize(this);

            Dispatcher.CurrentDispatcher.BeginInvoke(new System.Action(async () =>
            {
                try
                {
                    await Install();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

            }), DispatcherPriority.SystemIdle, null);
        }

        private async System.Threading.Tasks.Task Install()
        {
            var repository = (IVsExtensionRepository)GetService(typeof(SVsExtensionRepository));
            var manager = (IVsExtensionManager)GetService(typeof(SVsExtensionManager));
            var store = new DataStore();
            var missing = GetMissingExtensions(manager, store);

            if (!missing.Any())
                return;

            var allToBeInstalled = missing.ToArray();
            var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));

            var hwnd = new IntPtr(dte.MainWindow.HWnd);
            var window = (Window)System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual;

            var dialog = new InstallerProgress(missing, $"Downloading extensions...");
            dialog.Owner = window;
            dialog.Show();

            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (var product in allToBeInstalled)
                {
                    if (!dialog.IsVisible)
                        break; // User canceled the dialog

                    dialog.StartDownloading(product.Key);
                    dialog.SetMessage($"Installing {product.Value}...");
                    InstallExtension(repository, manager, product, store);
                    dialog.InstallComplete(product.Key);
                }

                store.Save();
            });

            if (dialog != null && dialog.IsVisible)
            {
                dialog.Close();
                dialog = null;
                PromptForRestart(allToBeInstalled.Select(ext => ext.Value));
            }
        }

        private void InstallExtension(IVsExtensionRepository repository, IVsExtensionManager manager, KeyValuePair<string, string> product, DataStore store)
        {
#if DEBUG
            System.Threading.Thread.Sleep(1000);
            return;
#endif

            GalleryEntry entry = null;

            try
            {
                entry = repository.CreateQuery<GalleryEntry>(includeTypeInQuery: false, includeSkuInQuery: true, searchSource: "ExtensionManagerUpdate")
                                                                                 .Where(e => e.VsixID == product.Key)
                                                                                 .AsEnumerable()
                                                                                 .FirstOrDefault();

                if (entry != null)
                {
                    var installable = repository.Download(entry);
                    manager.Install(installable, false);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                if (entry != null)
                {
                    store.PreviouslyInstalledExtensions.Add(entry.VsixID);
                }
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetMissingExtensions(IVsExtensionManager manager, DataStore store)
        {
            var installed = manager.GetInstalledExtensions();
            var products = ExtensionList.Products();
            var notInstalled = products.Where(product => !installed.Any(ins => ins.Header.Identifier == product.Key)).ToArray();

            return notInstalled.Where(ext => !store.HasBeenInstalled(ext.Key));

        }

        private void PromptForRestart(IEnumerable<string> extensions)
        {
            string list = string.Join(Environment.NewLine, extensions);
            string prompt = $"The following extensions were installed:\r\r{list}\r\rDo you want to restart Visual Studio now?";
            var answer = VsShellUtilities.ShowMessageBox(this, prompt, Vsix.Name, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);

            if (answer == (int)MessageBoxResult.OK)
            {
                IVsShell4 shell = (IVsShell4)GetService(typeof(SVsShell));
                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);
            }
        }
    }
}
