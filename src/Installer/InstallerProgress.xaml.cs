using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using WebExtensionPack.Controls;

namespace WebExtensionPack
{
    public partial class InstallerProgress : Window
    {
        public InstallerProgress(IEnumerable<KeyValuePair<string, string>> extensions, string message)
        {
            Loaded += delegate
            {
                Title = Vsix.Name;
                bar.Maximum = extensions.Count() + 1;
                bar.Value = 0;
                lblText.Content = message;

                foreach (var product in extensions)
                {
                    Extensions.Children.Add(new ExtensionItem(product.Key, product.Value));
                }

                Focus();
            };

            InitializeComponent();
        }

        public void SetMessage(string message)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                lblText.Content = message;
                bar.Value += 1;

            }), DispatcherPriority.Normal, null);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            Close();
        }

        public void StartDownloading(string key)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var child in Extensions.Children)
                {
                    var extension = (ExtensionItem)child;
                    if (extension.ExtensionGuid == key)
                    {
                        extension.StartDownloading();
                        break;
                    }
                }
            });
        }

        public void InstallComplete(string key)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var child in Extensions.Children)
                {
                    var extension = (ExtensionItem)child;
                    if (extension.ExtensionGuid == key)
                    {
                        extension.SetAsComplete();
                        break;
                    }
                }
            });
        }
    }
}
