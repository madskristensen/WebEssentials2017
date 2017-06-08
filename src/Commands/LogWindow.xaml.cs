using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ExtensionManager;
using System;
using System.Linq;
using System.Windows;

namespace WebEssentials.Commands
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : System.Windows.Window
    {
        private DTE2 _dte;
        private IVsExtensionRepository _repository;
        private IVsExtensionManager _manager;

        public LogWindow(DTE2 dte, IVsExtensionRepository repository, IVsExtensionManager manager)
        {
            InitializeComponent();

            _dte = dte;
            _repository = repository;
            _manager = manager;

            Loaded += (s, e) =>
            {
                Title = Vsix.Name;

                description.Text = "The Experimental Web Tools contain experimental features from the Visual Studio Web Team.";

                var logs = InstallerPackage.Installer.Store.Log.Select(l => l.ToString());
                log.Text = string.Join(Environment.NewLine, logs);

                reset.Content = "Reset...";
                reset.Click += ResetClickAsync;
            };
        }

        private async void ResetClickAsync(object sender, RoutedEventArgs e)
        {
            string msg = "This will update the list of experimental features and install all of them.\r\n\r\nDo you wish to continue?";
            var answer = MessageBox.Show(msg, Vsix.Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes)
                return;

            Telemetry.ResetInvoked();
            Close();

            try
            {
                _dte.StatusBar.Text = "Resetting Experimental Features...";
                _dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationGeneral);

                var vsVersion = InstallerPackage.GetVisualStudioVersion();
                await InstallerPackage.Installer.ResetAsync(vsVersion, _repository, _manager);
            }
            finally
            {
                _dte.StatusBar.Text = "Experimental Features have been reset";
                _dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationGeneral);
            }
        }
    }
}
