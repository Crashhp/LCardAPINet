using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LusbapiBridgeE2010;
using LCard.API.Interfaces;
using LCard.E2010GUI.Startup;
using Autofac;
using LCard.Core.Extensions;
using LCard.Core.Enums;

namespace LCard.E2010GUI.ViewModels.Actions
{
    public class DeviceInformationViewModel : PropertyChangedBase
    {
        
        public class DevicePropertyPoco : Screen
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public DevicePropertyGroup Group { get; set; }
        }

        private M_MODULE_DESCRIPTION_E2010 moduleDescription;

        public IObservableCollection<DevicePropertyPoco> _deviceProp;
        public IObservableCollection<DevicePropertyPoco> DeviceProp
        {
            get { return _deviceProp; }
            set
            {
                _deviceProp = value;
                NotifyOfPropertyChange(() => DeviceProp);
            }
        }


        public DeviceInformationViewModel()
        {
            _deviceProp = new BindableCollection<DevicePropertyPoco>();
        }

        public void LoadDeviceInfo()
        {
            DeviceProp.Clear();
            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            if(e2020.Inited == false) e2020.Init();
            moduleDescription = e2020.GET_MODULE_DESCRIPTION();

            //ADC
            var adc = moduleDescription.Adc;
            AddParameter("Active", adc.Active.ToString());
            AddParameter("Название модуля", adc.Name);
            AddParameter("Комментарий", adc.Comment);
            AddParameter("Калибровочные коэфициенты", "");
            int i = 0;
            foreach (var coeff in adc.ScaleCalibration)
            {
                AddParameter("  Калибр. коэфициент " + i, $"{coeff:N2}");
                i++;
                if (i > 12) break;
            }
            AddParameter("Коэфициенты смещения", "");
            i = 0;
            foreach (var coeff in adc.OffsetCalibration)
            {
                AddParameter("  Смещ. коэфициент " + i, $"{coeff:N2}");
                i++;
                if (i > 12) break;
            }
            //interface
            var linterface = moduleDescription.Module;
            AddParameter("Коменнатарий", linterface.Comment, DevicePropertyGroup.Interface);
            AddParameter("Имя компании", linterface.CompanyName, DevicePropertyGroup.Interface);

            //Module
            var module = moduleDescription.Module;
            AddParameter("Имя устройства", module.DeviceName, DevicePropertyGroup.Module);
            AddParameter("Ревизия", Convert.ToChar(module.Revision).ToString(), DevicePropertyGroup.Module);
            AddParameter("Серийный номер", (module.SerialNumber), DevicePropertyGroup.Module);
            AddParameter("Модификация", module.Modification.ToString(), DevicePropertyGroup.Module);

            //Module
            var mcu = moduleDescription.Mcu;

        }

        private void AddParameter(string name, string value, DevicePropertyGroup group = DevicePropertyGroup.Adc)
        {
            DeviceProp.Add(new DevicePropertyPoco { Name = name, Value = value, Group = group });
        }

        
        
    }
}
