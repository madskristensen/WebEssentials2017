using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebExtensionPack.Controls
{
    /// <summary>
    /// Interaction logic for ExtensionItem.xaml
    /// </summary>
    public partial class ExtensionItem : UserControl
    {
        public string ExtensionGuid { get; }

        public ExtensionItem(string extensionGuid, string extensionName)
        {
            InitializeComponent();

            this.ExtensionGuid = extensionGuid;
            this.ExtensionName.Text = extensionName;
        }

        public void StartDownloading()
        {
            GridTick.Visibility = Visibility.Collapsed;
            GridPending.Visibility = Visibility.Collapsed;
            GridLoading.Visibility = Visibility.Visible;
        }

        public void SetAsComplete()
        {
            GridPending.Visibility = Visibility.Collapsed;
            GridLoading.Visibility = Visibility.Collapsed;
            GridTick.Visibility = Visibility.Visible;
        }
    }
}
