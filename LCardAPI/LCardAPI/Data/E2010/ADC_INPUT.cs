using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // возможные типы подключения входного тракта модуля E20-10
    public enum ADC_INPUT : ushort
    {
        ADC_INPUT_ZERO_E2010, 
        ADC_INPUT_SIGNAL_E2010, 
        INVALID_ADC_INPUT_E2010
    };
}
