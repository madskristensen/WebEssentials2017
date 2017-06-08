using EnvDTE;
using EnvDTE80;
using WebEssentials.Commands;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Windows.Interop;

namespace WebEssentials
{
    internal sealed class ShowModalCommand
    {
        private readonly AsyncPackage _package;

        private ShowModalCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package;

            var menuCommandID = new CommandID(PackageGuids.guidVSPackageCmdSet, PackageIds.ResetExtensions);
            var menuItem = new MenuCommand(this.ResetAsync, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static ShowModalCommand Instance
        {
            get;
            private set;
        }

        public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ShowModalCommand(package, commandService);
        }

        private async void ResetAsync(object sender, EventArgs e)
        {
            var dte = await _package.GetServiceAsync(typeof(DTE)) as DTE2;
            var repository = await _package.GetServiceAsync(typeof(SVsExtensionRepository)) as IVsExtensionRepository;
            var manager = await _package.GetServiceAsync(typeof(SVsExtensionManager)) as IVsExtensionManager;

            var dialog = new LogWindow(dte, repository, manager);

            var hwnd = new IntPtr(dte.MainWindow.HWnd);
            var window = (System.Windows.Window)HwndSource.FromHwnd(hwnd).RootVisual;
            dialog.Owner = window;

            var result = dialog.ShowDialog();
        }
    }
}
