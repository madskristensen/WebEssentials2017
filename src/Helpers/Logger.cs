using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using WebEssentials;

internal static class Logger
{
    private static IVsOutputWindowPane _pane;
    private static IVsOutputWindow _output = (IVsOutputWindow)ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow));

    public static void Log(string message, bool addNewLine = true)
    {
        ThreadHelper.Generic.BeginInvoke(() =>
        {
            try
            {
                if (EnsurePane())
                {
                    if (addNewLine)
                    {
                        message += Environment.NewLine;
                    }

                    _pane.OutputString(message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        });
    }

    public static void ShowOutputWindowPane()
    {
        ThreadHelper.Generic.BeginInvoke(() =>
        {
            if (EnsurePane())
            {
                VsHelpers.ShowOutputWindow();
                _pane.Activate();
            }
        });
    }

    private static bool EnsurePane()
    {
        if (_pane == null)
        {
            var guid = Guid.NewGuid();
            _output.CreatePane(ref guid, Vsix.Name, 1, 1);
            _output.GetPane(ref guid, out _pane);
        }

        return _pane != null;
    }
}