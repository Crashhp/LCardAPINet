using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using LCard.API.Interfaces;
using LCard.Manager.Properties;
using LCard.Manager.Startup;
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

            var deviceManager = UnityConfig.GetConfiguredContainer().Resolve<IDeviceManager>();
            deviceManager.mE2010 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            deviceManager.StartDetectionLoop();
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                Settings.Default.IsBlockAdapter = deviceManager.IsBlockAdapter;
                Settings.Default.Save();
            });
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.DataContext = new MainWindowViewModel(new DialogService(this), this.DataView.WindowsFormsHostGraphData);
        }
    }
}
