using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using Autofac;
using LCard.API.Interfaces;
using LCard.Manager.Enums;
using LCard.Manager.Properties;
using LCard.Manager.Startup;

namespace LCard.Manager.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly WindowsFormsHost windowsFormsHostGraph;
        private SelectedMainTabItem selectedTabIndex = SelectedMainTabItem.DataView;

        public InformationViewModel InformationViewModel { get; set; }
        public DataViewModel DataViewModel { get; set; }
        public SettingsViewModel SettingsViewModel { get; set; }

        public MainWindowViewModel(IDialogService dialogService, WindowsFormsHost windowsFormsHostGraph)
        {
            this.dialogService = dialogService;
            this.windowsFormsHostGraph = windowsFormsHostGraph;
            Initializate();
            var module = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            if (!module.OpenLDevice())
            {
                this.dialogService.ShowMessage("","Устройство не подключено");
            }
        }

        public string Version
        {
            get { return "Версия " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public int SelectedTabIndex
        {
            get { return (int)selectedTabIndex; }
            set
            {
                if (value == (int)selectedTabIndex) return;
                selectedTabIndex = (SelectedMainTabItem)value;
                OnPropertyChanged();
                if ((SelectedMainTabItem)SelectedTabIndex == SelectedMainTabItem.InformationView)
                {
                    Settings.Default.Reload();
                    InformationViewModel.LoadDeviceInfo();
                }
                if ((SelectedMainTabItem)SelectedTabIndex == SelectedMainTabItem.DataView)
                {
                    Settings.Default.Reload();
                    DataViewModel.PrepareGraph();
                }
            }
        }

        private void Initializate()
        {
            InformationViewModel = new InformationViewModel();
            DataViewModel = new DataViewModel(windowsFormsHostGraph, this.dialogService);
            SettingsViewModel = new SettingsViewModel(() =>
            {
                
            }, this.dialogService);
        }
    }
}
