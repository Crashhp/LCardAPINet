using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // доступные индексы диапазонов входного напряжения модуля E20-10
    public enum ADC_INPUTV : ushort
    {
        ADC_INPUT_RANGE_3000mV_E2010, 
        ADC_INPUT_RANGE_1000mV_E2010, 
        ADC_INPUT_RANGE_300mV_E2010, 
        INVALID_ADC_INPUT_RANGE_E2010
    };
}
