using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // доступные индексы источника сигнала старта сбора данных
    public enum INT_ADC_START : ushort
    {
        INT_ADC_START_E2010, INT_ADC_START_WITH_TRANS_E2010,
        EXT_ADC_START_ON_RISING_EDGE_E2010, EXT_ADC_START_ON_FALLING_EDGE_E2010,
        //EXT_ADC_START_ON_HIGH_LEVEL_E2010, EXT_ADC_START_ON_LOW_LEVEL_E2010,        // для Rev.B и выше (пока нет)
        INVALID_ADC_START_E2010
    };
}
