using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace LCard.Manager.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly WindowsFormsHost windowsFormsHostGraph;
        private int selectedTabIndex = 0;

        public InformationViewModel InformationViewModel { get; set; }
        public DataViewModel DataViewModel { get; set; }
        public SettingsViewModel SettingsViewModel { get; set; }

        public MainWindowViewModel(IDialogService dialogService, WindowsFormsHost windowsFormsHostGraph)
        {
            this.dialogService = dialogService;
            this.windowsFormsHostGraph = windowsFormsHostGraph;
            Initializate();
        }

        public string Version
        {
            get { return "Версия " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                if (value == selectedTabIndex) return;
                selectedTabIndex = value;
                OnPropertyChanged();
                if (SelectedTabIndex == 2)
                {
                    InformationViewModel.LoadDeviceInfo();
                }
                if (SelectedTabIndex == 0)
                {
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
