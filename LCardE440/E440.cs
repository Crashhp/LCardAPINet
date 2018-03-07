using LCardE440Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCardE440
{
    class E440
    {
        private E440Bridge _E440Bridge;
        private double _AdcRate;
        private TimeSpan _Duration;

        private IntPtr _Handle;
        private string _Name;
        private byte _UsbSpeed;
        private M_MODULE_DESCRIPTION_E440 _ModuleDescription;

        public E440(double adcRate, TimeSpan duration)
        {
            _E440Bridge = new E440Bridge();
            var dll = _E440Bridge.DllVersion();
            var ins = _E440Bridge.CreateInstance();

            _AdcRate = adcRate;
            _Duration = duration;
        }

        public void ReadData()
        {
            var open = _E440Bridge.OpenLDevice(0);
            _Handle = _E440Bridge.GetModuleHandleDevice();
            _Name = _E440Bridge.GetModuleName();
            _UsbSpeed = _E440Bridge.GetUsbSpeed();
            var load = _E440Bridge.LoadModule();
            var test = _E440Bridge.TestModule();
            _ModuleDescription = _E440Bridge.GetModuleDescription();
            _E440Bridge.Dispose();
        }
    }
}
