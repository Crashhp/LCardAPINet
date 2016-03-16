using System;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using LCard.API.Data.E2010;
using LCard.API.Interfaces;
using LCard.Core.Logger;
using LCard.Manager.Properties;
using LCard.Manager.Startup;
using MahApps.Metro.Controls.Dialogs;

namespace LCard.Manager.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        private double frequency = Settings.Default.InputRateInkHz;
        private readonly Action closeAction;
        private readonly IDialogService dialogService;

        public RelayCommand SetDeviceSettingsCommand { get; private set; }
        public RelayCommand SetDefaultDeviceSettingsCommand { get; private set; }
        public RelayCommand SetSaveResultPathCommand { get; private set; }
        public string[] IncreasedInputTypes { get; set; }
        public ADC_INPUTV ADC_INPUTV { get; set; }
        public double Frequency
        {
            get { return frequency; }
            set
            {
                if (value == frequency) return;
                frequency = value;
                Settings.Default.InputRateInkHz = value;
                OnPropertyChanged();
            }
        }

        private string increasedInputType = "0.3";
        public string IncreasedInputType
        {
            get
            {
                return increasedInputType;
            }
            set
            {
                increasedInputType = value;
                if (increasedInputType == IncreasedInputTypes[0])
                {
                    ADC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_300mV_E2010;
                    Settings.Default.InputRange = 0;
                }
                if (increasedInputType == IncreasedInputTypes[1])
                {
                    ADC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_1000mV_E2010;
                    Settings.Default.InputRange = 1;
                }
                if (increasedInputType == IncreasedInputTypes[2])
                {
                    ADC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_3000mV_E2010;
                    Settings.Default.InputRange = 2;
                }
                OnPropertyChanged();
            }
        }

        public SettingsViewModel(Action closeAction, IDialogService dialogService)
        {
            this.closeAction = closeAction;
            this.dialogService = dialogService;
            SetDeviceSettingsCommand = new RelayCommand(_ => SetNewDevSettings());
            SetDefaultDeviceSettingsCommand = new RelayCommand(_ => SetDefaultSettings());
            SetSaveResultPathCommand = new RelayCommand(_ => SelectNewSaveResultPath());
            IncreasedInputTypes = new[] { "0.3", "1.0", "3.0"};
        }

        protected void SelectNewSaveResultPath()
        {
            var folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Settings.Default.SaveResultPath = folderDialog.SelectedPath;
            }
        }

        protected async void SetNewDevSettings()
        {
            var result = await this.dialogService.AskQuestionAsync("", "Установить новые параметры?");
            if (result == MessageDialogResult.Affirmative)
            {
                Logger.Current.Info("Set Adc Params");
                var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
                if (e2020.Inited == false) e2020.Init();
                e2020.AdcRateInKhz = Settings.Default.InputRateInkHz;
                e2020.InputRange = (ADC_INPUTV) Settings.Default.InputRange;
                e2020.SetParameters();
                Settings.Default.Save();
                closeAction();
            }
        }

        protected async void SetDefaultSettings()
        {
            var result = await this.dialogService.AskQuestionAsync("", "Установить параметры по умолчанию?");
            if (result == MessageDialogResult.Affirmative)
            {
                Logger.Current.Info("Set Default Adc Params");
                var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
                if (e2020.Inited == false) e2020.Init();
                Frequency = 100.0;
                IncreasedInputType = IncreasedInputTypes[0];
                e2020.AdcRateInKhz = Settings.Default.InputRateInkHz;
                e2020.InputRange = (ADC_INPUTV)Settings.Default.InputRange;
                e2020.SetParameters();

                Settings.Default.Save();
                closeAction();
            }
            
        }

        public static int NumberSelectedChannels
        {
            get
            {
                return new[]
                {
                    Settings.Default.IsChannel1,
                    Settings.Default.IsChannel2,
                    Settings.Default.IsChannel3,
                    Settings.Default.IsChannel4
                }.Where(v => v).Count();
            }
        }


    }
}
