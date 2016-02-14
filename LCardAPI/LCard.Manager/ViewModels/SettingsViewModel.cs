using System;

namespace LCard.Manager.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        private int frequency = 5000;
        private readonly Action closeAction;
        public SettingsViewModel(Action closeAction)
        {
            this.closeAction = closeAction;
            SetDeviceSettingsCommand = new RelayCommand(_ => SetNewDevSettings());
            SetDefaultDeviceSettingsCommand = new RelayCommand(_ => SetDefaultSettings());
        }
        public RelayCommand SetDeviceSettingsCommand { get; private set; }
        public RelayCommand SetDefaultDeviceSettingsCommand { get; private set; }

        public int Frequency
        {
            get { return frequency; }
            set
            {
                if (value == frequency) return;
                frequency = value;
                OnPropertyChanged();
            }
        }

        protected void SetNewDevSettings()
        {
            closeAction();
        }

        protected void SetDefaultSettings()
        {
            closeAction();
        }
    }
}
