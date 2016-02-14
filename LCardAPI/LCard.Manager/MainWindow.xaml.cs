using System.Threading;
using System.Windows;
using LCard.Manager.ViewModels;
using MahApps.Metro.Controls;

namespace LCard.Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.DataContext = new MainWindowViewModel(new DialogService(this), this.DataView.WindowsFormsHostGraphData);
        }
    }
}
