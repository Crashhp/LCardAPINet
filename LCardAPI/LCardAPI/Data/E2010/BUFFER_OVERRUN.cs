using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    public enum BUFFER_OVERRUN : ushort
    {
        // битовое поле BufferOverrun структуры DATA_STATE_E2010
        BUFFER_OVERRUN_E2010 = 0x0,        // переполнение внутреннего буфера модуля
        // битовое поле ChannelsOverFlow структуры DATA_STATE_E2010 (для Rev.B и выше)
        OVERFLOW_OF_CHANNEL_1_E2010 = 0x0, OVERFLOW_OF_CHANNEL_2_E2010,    // биты локальных признаков переполнения разрядной сетки соответствующего канала
        OVERFLOW_OF_CHANNEL_3_E2010, OVERFLOW_OF_CHANNEL_4_E2010,            // за время выполнения одного запроса сбора данных ReadData()
        OVERFLOW_E2010 = 0x7                    // бит глобального признака факта переполнения разрядной сетки модуля за всё время сбора данных от момента START_ADC() до STOP_ADC()
    };
}
