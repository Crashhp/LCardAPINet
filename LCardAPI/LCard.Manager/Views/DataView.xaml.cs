using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static LCard.Manager.Properties.Settings;

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
                Default.IsChannel1 == false &&
                Default.IsChannel2 == false &&
                Default.IsChannel3 == false &&
                Default.IsChannel4 == false)
            {
                
            }
            Default.Save();
        }

        private int GetNumberOfActiveChannles()
        {
            return new[]
            {
                Settings.Default.IsChannel1,
                Settings.Default.IsChannel2,
                Settings.Default.IsChannel3,
                Settings.Default.IsChannel4
            }.Where(v => v == true).Count();
        }

        private void ToggleButton_OnChecked1(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsChannel1 = true;
            ControlDynamicGraph.EnabelChannel(0);
            Default.Save();
        }

        private void ToggleButton_OnChecked2(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsChannel2 = true;
            ControlDynamicGraph.EnabelChannel(1);
            Default.Save();
        }

        private void ToggleButton_OnChecked3(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsChannel3 = true;
            ControlDynamicGraph.EnabelChannel(2);
            Default.Save();
        }

        private void ToggleButton_OnChecked4(object sender, RoutedEventArgs e)
        {
            Settings.Default.IsChannel4 = true;
            ControlDynamicGraph.EnabelChannel(3);
            Default.Save();
        }

        private void ToggleButton_OnUnchecked1(object sender, RoutedEventArgs e)
        {
            if (GetNumberOfActiveChannles() == 0)
            {
                Settings.Default.IsChannel1 = true;
            }
            else
            {
                ControlDynamicGraph.DisableChannel(0);
            }
            Default.Save();
        }

        private void ToggleButton_OnUnchecked2(object sender, RoutedEventArgs e)
        {
            if (GetNumberOfActiveChannles() == 0)
            {
                Settings.Default.IsChannel2 = true;
            }
            else
            {
                ControlDynamicGraph.DisableChannel(1);
            }
            Default.Save();
        }

        private void ToggleButton_OnUnchecked3(object sender, RoutedEventArgs e)
        {
            if (GetNumberOfActiveChannles() == 0)
            {
                Settings.Default.IsChannel3 = true;
            }
            else
            {
                ControlDynamicGraph.DisableChannel(2);
            }
            Default.Save();
        }

        private void ToggleButton_OnUnchecked4(object sender, RoutedEventArgs e)
        {
            if (GetNumberOfActiveChannles() == 0)
            {
                Settings.Default.IsChannel4 = true;
            }
            else
            {
                ControlDynamicGraph.DisableChannel(3);
            }
            Default.Save();
        }

        private void View_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Write_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
