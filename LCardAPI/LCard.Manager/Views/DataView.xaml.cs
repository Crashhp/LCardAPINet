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
using LCard.Manager.Properties;
using LCard.Manager.ViewModels;

namespace LCard.Manager.Views
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataView : UserControl
    {
        public DataView()
        {
            InitializeComponent();
        }

        private void ChannelEnabled_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                Settings.Default.IsChannel1 == false &&
                Settings.Default.IsChannel2 == false &&
                Settings.Default.IsChannel3 == false &&
                Settings.Default.IsChannel4 == false)
            {
                
            }
        }
    }
}
