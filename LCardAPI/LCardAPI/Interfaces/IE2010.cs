using System;
using System.Collections.Generic;
using LCard.API.Data;
using LCard.API.Data.E2010;
using LCard.Core.Poco;
using LusbapiBridgeE2010;

namespace LCard.API.Interfaces
{
    public interface IE2010
    {
        double AdcRateInKhz { get; set; }
        ADC_INPUTV InputRange { get; set; }

        bool OpenLDevice();
        IntPtr GetModuleHandleDevice();
        string GetModuleName();
        LusbSpeed GetUsbSpeed();
        bool LOAD_MODULE();
        bool TEST_MODULE();

        bool Inited { get; set; }

        M_MODULE_DESCRIPTION_E2010 GET_MODULE_DESCRIPTION();

        M_ADC_PARS_E2010 GET_ADC_PARS();

        void SET_ADC_PARS(M_ADC_PARS_E2010 AdcPars, int DataStep);

        M_MODULE_DESCRIPTION_E2010? Init();

        bool StartReadData();

        Action<DataPacketPoco> OnData { get; set; }
 
        bool StopReadData();

        void SetParameters();
    }
}
