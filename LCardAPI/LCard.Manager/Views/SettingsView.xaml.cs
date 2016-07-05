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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            UpdateAdapterValue();
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            LabelBlockAdapter.Content = "Блок адаптера присутствует";
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            LabelBlockAdapter.Content = "Блок адаптера отсутствует";
        }

        private void UpdateAdapterValue()
        {
            if (Settings.Default.IsBlockAdapter)
            {
                LabelBlockAdapter.Content = "Блок адаптера присутствует";
            }
            else
            {
                LabelBlockAdapter.Content = "Блок адаптера отсутствует";
            }
        }
    }
}
