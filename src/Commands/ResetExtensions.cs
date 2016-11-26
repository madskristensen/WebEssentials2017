using System;
using System.ComponentModel.Design;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebExtensionPack
{
    internal sealed class ResetExtensions
    {
        private readonly Package _package;

        private ResetExtensions(Package package)
        {
            _package = package;

            OleMenuCommandService commandService = (OleMenuCommandService)ServiceProvider.GetService(typeof(IMenuCommandService));
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.guidVSPackageCmdSet, PackageIds.ResetExtensions);
                var menuItem = new MenuCommand(Execute, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        public static ResetExtensions Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new ResetExtensions(package);
        }

        private void Execute(object sender, EventArgs e)
        {
            string message = "This will reset Web Extension Pack and restart Visual Studio.\r\n\r\nDo you wish to continue?";

            var answer = VsShellUtilities.ShowMessageBox(
                ServiceProvider,
                message,
                Vsix.Name,
                OLEMSGICON.OLEMSGICON_QUERY,
                OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            if (answer == (int)MessageBoxResult.OK)
            {
                var store = new DataStore();

                if (store.Reset())
                {
                    IVsShell4 shell = (IVsShell4)ServiceProvider.GetService(typeof(SVsShell));
                    shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);
                }
                else
                {
                    var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
                    dte.StatusBar.Text = "An error occured. Please see output window for details.";
                }
            }
        }
    }
}
