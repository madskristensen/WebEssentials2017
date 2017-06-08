using Microsoft.VisualStudio.ExtensionManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TaskStatusCenter;

namespace WebEssentials
{
    public class Installer
    {
        public Installer(LiveFeed feed, DataStore store)
        {
            LiveFeed = feed;
            Store = store;
        }

        public DataStore Store { get; }

        public LiveFeed LiveFeed { get; }

        public async Task<bool> CheckForUpdatesAsync()
        {
            var file = new FileInfo(LiveFeed.LocalCachePath);
            bool hasUpdates = false;

            if (!file.Exists || file.LastWriteTime < DateTime.Now.AddDays(-Constants.UpdateIntervalDays))
            {
                hasUpdates = await LiveFeed.UpdateAsync();
            }
            else
            {
                await LiveFeed.ParseAsync();
            }

            return hasUpdates;
        }

        public async Task ResetAsync(Version vsVersion, IVsExtensionRepository repository, IVsExtensionManager manager)
        {
            Store.Reset();
            await LiveFeed.UpdateAsync();
            await RunAsync(vsVersion, repository, manager, default(CancellationToken));
        }

        public async Task RunAsync(Version vsVersion, IVsExtensionRepository repository, IVsExtensionManager manager, CancellationToken cancellationToken)
        {
            IEnumerable<ExtensionEntry> toUninstall = GetExtensionsMarkedForDeletion(vsVersion);
            IEnumerable<ExtensionEntry> toInstall = GetMissingExtensions(manager).Except(toUninstall);

            int actions = toUninstall.Count() + toInstall.Count();

            if (actions > 0)
            {
                Begin?.Invoke(this, actions);

                await UninstallAsync(toUninstall, repository, manager, cancellationToken);
                await InstallAsync(toInstall, repository, manager, cancellationToken);

                Done?.Invoke(this, actions);
            }
        }

        private async Task InstallAsync(IEnumerable<ExtensionEntry> extensions, IVsExtensionRepository repository, IVsExtensionManager manager, CancellationToken token)
        {
            if (!extensions.Any() || token.IsCancellationRequested)
                return;

            //#if DEBUG
            //            // Don't install while running in debug mode
            //            foreach (var ext in extensions)
            //            {
            //                await Task.Delay(2000);
            //                Store.MarkInstalled(ext);
            //            }
            //            Store.Save();
            //            return;
            //#endif

            await Task.Run(() =>
            {
                try
                {
                    foreach (ExtensionEntry extension in extensions)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        InstallExtension(extension, repository, manager);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }
                finally
                {
                    Store.Save();
                }
            });
        }

        private async Task UninstallAsync(IEnumerable<ExtensionEntry> extensions, IVsExtensionRepository repository, IVsExtensionManager manager, CancellationToken token)
        {
            if (!extensions.Any() || token.IsCancellationRequested)
                return;

            await Task.Run(() =>
            {
                try
                {
                    foreach (ExtensionEntry ext in extensions)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        UnInstalling?.Invoke(this, ext.Name);

                        try
                        {
                            if (manager.TryGetInstalledExtension(ext.Id, out IInstalledExtension installedExtension))
                            {
                                manager.Uninstall(installedExtension);
                                Store.MarkUninstalled(ext);
                                Telemetry.Uninstall(ext.Id, true);
                            }
                        }
                        catch (Exception)
                        {
                            Telemetry.Uninstall(ext.Id, false);
                        }
                    }
                }
                finally
                {
                    Store.Save();
                }
            });
        }

        private void InstallExtension(ExtensionEntry extension, IVsExtensionRepository repository, IVsExtensionManager manager)
        {
            GalleryEntry entry = null;
            Installing?.Invoke(this, extension.Name);

            try
            {
                entry = repository.GetVSGalleryExtensions<GalleryEntry>(new List<string> { extension.Id }, 1033, false)?.FirstOrDefault();

                if (entry != null)
                {
                    IInstallableExtension installable = repository.Download(entry);
#if DEBUG
                    return;
#endif
                    manager.Install(installable, false);
                    Telemetry.Install(extension.Id, true);
                }
            }
            catch (Exception)
            {
                Telemetry.Install(extension.Id, false);
            }
            finally
            {
                if (entry != null)
                {
                    Store.MarkInstalled(extension);
                }
            }
        }

        private IEnumerable<ExtensionEntry> GetMissingExtensions(IVsExtensionManager manager)
        {
            IEnumerable<IInstalledExtension> installed = manager.GetInstalledExtensions();
            IEnumerable<ExtensionEntry> notInstalled = LiveFeed.Extensions.Where(ext => !installed.Any(ins => ins.Header.Identifier == ext.Id));

            return notInstalled.Where(ext => !Store.HasBeenInstalled(ext.Id));
        }

        internal IEnumerable<ExtensionEntry> GetExtensionsMarkedForDeletion(Version VsVersion)
        {
            return LiveFeed.Extensions.Where(ext => ext.MinVersion > VsVersion || ext.MaxVersion < VsVersion);
        }

        public event EventHandler<int> Begin;
        public event EventHandler<int> Done;
        public event EventHandler<string> Installing;
        public event EventHandler<string> UnInstalling;
    }
}
