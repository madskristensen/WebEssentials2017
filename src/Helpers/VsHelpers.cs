using System;
using System.Windows;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebEssentials
{
    public static class VsHelpers
    {
        public static DTE2 DTE = Package.GetGlobalService(typeof(DTE)) as DTE2;

        public static void ShowTaskStatusCenter()
        {
            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ApplicationIdle, () => {

                Command cmd = DTE.Commands.Item("View.ShowTaskStatusCenter");

                if (cmd != null && cmd.IsAvailable)
                {
                    DTE.Commands.Raise(cmd.Guid, cmd.ID, null, null);
                }
            });
        }

        public static Version GetVisualStudioVersion()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.FileVersionInfo v = process.MainModule.FileVersionInfo;

            return new Version(v.ProductMajorPart, v.ProductMinorPart, v.ProductBuildPart);
        }

        public static void PromptForRestart()
        {
            string prompt = $"Extensions have been installed. Visual Studio must be restarted for the changes to take effect.\r\rDo you want to restart Visual Studio now?";
            int answer = VsShellUtilities.ShowMessageBox(ServiceProvider.GlobalProvider, prompt, Vsix.Name, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);

            if (answer == (int)MessageBoxResult.OK)
            {
                var shell = (IVsShell4)Package.GetGlobalService(typeof(SVsShell));
                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);
            }
        }
    }
}
