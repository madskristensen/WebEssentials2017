using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace WebEssentials
{
    public static class VsHelpers
    {
        public static DTE2 DTE = Package.GetGlobalService(typeof(DTE)) as DTE2;

        public static void ShowTaskStatusCenter()
        {
            ThreadHelper.Generic.BeginInvoke(() => {

                Command cmd = DTE.Commands.Item("View.ShowTaskStatusCenter");

                if (cmd != null && cmd.IsAvailable)
                {
                    DTE.Commands.Raise(cmd.Guid, cmd.ID, null, null);
                }
            });
        }
    }
}
