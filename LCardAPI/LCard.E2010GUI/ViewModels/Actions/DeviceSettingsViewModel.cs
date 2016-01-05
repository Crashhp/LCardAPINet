using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro;
using LCard.API.Data;
using LCard.API.Data.E2010;
using LCard.API.Interfaces;
using LCard.Core.Logger;
using LCard.E2010GUI.Startup;
using LusbapiBridgeE2010;

namespace LCard.E2010GUI.ViewModels.Actions
{
    public class DeviceSettingsViewModel : PropertyChangedBase
    {
        private ADC_INPUTV aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_300mV_E2010;

        private string _selectedRangeIndex = "0.3, V";

        public List<string> RangeIndex
        {
            get
            {
                return new List<string> { "0.3, V", "1.0, V", "3.0, V" };
            }
        }

        private string _inputRatekHz =  ModuleE2010.Default.InputRateInkHz.ToString("0.#");

        public string InputRatekHz
        {
            get { return _inputRatekHz; }
            set
            {
                double rateKhz;
                if (double.TryParse(value, out rateKhz))
                {
                    _inputRatekHz = value;
                    ModuleE2010.Default.InputRateInkHz = rateKhz;
                    NotifyOfPropertyChange(() => InputRatekHz);
                }
                
                
            }
        }

        public string SelectedRangeIndex
        {
            get
            {
                return _selectedRangeIndex;
            }
            set
            {
                _selectedRangeIndex = value;
                if (_selectedRangeIndex == RangeIndex[0]) {
                    aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_300mV_E2010;
                    ModuleE2010.Default.InputRange = 0;
                }
                if (_selectedRangeIndex == RangeIndex[1])
                {
                    aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_1000mV_E2010;
                    ModuleE2010.Default.InputRange = 1;
                }
                if (_selectedRangeIndex == RangeIndex[2])
                {
                    aDC_INPUTV = ADC_INPUTV.ADC_INPUT_RANGE_3000mV_E2010;
                    ModuleE2010.Default.InputRange = 2;
                }
                NotifyOfPropertyChange(() => this.SelectedRangeIndex);
            }
        }

        public void ButtonSetDeviceSettingsDefault()
        {
            Logger.Current.Info("Set Default Adc Params");
            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            if (e2020.Inited == false) e2020.Init();
            ModuleE2010.Default.InputRateInkHz = 100.0;
            ModuleE2010.Default.InputRange = 0;
            e2020.AdcRateInKhz = ModuleE2010.Default.InputRateInkHz;
            e2020.InputRange = (ADC_INPUTV) ModuleE2010.Default.InputRange;
            e2020.SetParameters();
            SelectedRangeIndex = RangeIndex[0];
            

            ModuleE2010.Default.Save();
        }

        public void ButtonSetDeviceSettings()
        {
            Logger.Current.Info("Set Adc Params");
            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            if (e2020.Inited == false) e2020.Init();
            e2020.AdcRateInKhz = ModuleE2010.Default.InputRateInkHz;
            e2020.InputRange = (ADC_INPUTV)ModuleE2010.Default.InputRange;
            e2020.SetParameters();
            ModuleE2010.Default.Save();
        }


    }
}
