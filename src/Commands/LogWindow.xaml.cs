using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WebEssentials.Commands
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {

        public LogWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Title = Vsix.Name;

                description.Text = "The Experimental Web Tools contain experimental features from the Visual Studio Web Team.";

                IEnumerable<string> logs = InstallerService.Installer.Store.Log.Select(l => l.ToString()).Reverse();
                log.Text = string.Join(Environment.NewLine, logs);

                reset.Content = "Re-install...";
                reset.Click += ResetClickAsync;
            };
        }

        private async void ResetClickAsync(object sender, RoutedEventArgs e)
        {
            string msg = "This will reset the log and install all missing Web Essentials extensions.\r\n\r\nDo you wish to continue?";
            MessageBoxResult answer = MessageBox.Show(msg, Vsix.Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes)
                return;

            Telemetry.ResetInvoked();
            Close();

            try
            {
                await InstallerService.ResetAsync();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }
    }
}
