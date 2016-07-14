using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using LCard.API.Interfaces;
using LCard.Core.Enums;
using LCard.Manager.Startup;

namespace LCard.Manager.ViewModels
{
    class InformationViewModel : ViewModelBase
    {
        private ObservableCollection<DevicePropertyPoco> deviceProperties = new ObservableCollection<DevicePropertyPoco>();

        public class DevicePropertyPoco : ViewModelBase
        {
            private string name;
            private string value;

            public string Name
            {
                get { return name; }
                set
                {
                    if (value == name) return;
                    name = value;
                    OnPropertyChanged();
                }
            }

            public string Value
            {
                get { return value; }
                set
                {
                    if (value == this.value) return;
                    this.value = value;
                    OnPropertyChanged();
                }
            }

            public DevicePropertyGroup Group { get; set; }
        }

        public ObservableCollection<DevicePropertyPoco> DeviceProperties
        {
            get { return deviceProperties; }
            set
            {
                if (Equals(value, deviceProperties)) return;
                deviceProperties = value;
                OnPropertyChanged();
            }
        }

        public InformationViewModel()
        {
            
        }

        public void LoadDeviceInfo()
        {
            DeviceProperties.Clear();
            var e2020 = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            if (e2020.Inited == false) e2020.Init();
            var moduleDescription = e2020.GET_MODULE_DESCRIPTION();

            //Module
            var module = moduleDescription.Module;
            AddParameter("Имя устройства", module.DeviceName, DevicePropertyGroup.Module);
            AddParameter("Ревизия", Convert.ToChar(module.Revision).ToString(), DevicePropertyGroup.Module);
            AddParameter("Серийный номер", (module.SerialNumber), DevicePropertyGroup.Module);
            AddParameter("Модификация", module.Modification.ToString(), DevicePropertyGroup.Module);
            AddParameter("Имя компании", module.CompanyName, DevicePropertyGroup.Interface);
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




            //Module
            var mcu = moduleDescription.Mcu;

            AddParameter("Напряжение питания", "");
            var deviceManager = UnityConfig.GetConfiguredContainer().Resolve<IDeviceManager>();
            for (i = 0; i < 4; i++)
            {
                AddParameter("Напряжение питания "+ (i + 1), deviceManager.BlockAdapterValues[i].ToString("N5"));
            }

        }

        private void AddParameter(string name, string value, DevicePropertyGroup group = DevicePropertyGroup.Adc)
        {
            DeviceProperties.Add(new DevicePropertyPoco { Name = name, Value = value, Group = group });
        }
    }
}
