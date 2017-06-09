using System;
using System.ComponentModel.Design;
using System.Windows.Interop;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using WebEssentials.Commands;

namespace WebEssentials
{
    internal sealed class ShowModalCommand
    {
        private readonly AsyncPackage _package;

        private ShowModalCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package;

            var menuCommandID = new CommandID(PackageGuids.guidVSPackageCmdSet, PackageIds.ResetExtensions);
            var menuItem = new MenuCommand(ResetAsync, menuCommandID);
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
            var dialog = new LogWindow();

            var hwnd = new IntPtr(dte.MainWindow.HWnd);
            var window = (System.Windows.Window)HwndSource.FromHwnd(hwnd).RootVisual;
            dialog.Owner = window;
            dialog.ShowDialog();
        }
    }
}
