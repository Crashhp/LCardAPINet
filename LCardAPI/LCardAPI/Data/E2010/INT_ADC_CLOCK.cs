using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // доступные индексы источника тактовых импульсов для запуска АЦП
    public enum INT_ADC_CLOCK : ushort
    {
        INT_ADC_CLOCK_E2010, 
        INT_ADC_CLOCK_WITH_TRANS_E2010, 
        EXT_ADC_CLOCK_ON_RISING_EDGE_E2010, 
        EXT_ADC_CLOCK_ON_FALLING_EDGE_E2010, 
        INVALID_ADC_CLOCK_E2010
    };
    
}
