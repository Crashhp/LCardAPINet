using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // возможные типы аналоговой синхронизации ввода данных (для Rev.B и выше)
    public enum E2010_SYNC : ushort
    {
        NO_ANALOG_SYNCHRO_E2010,            // отсутствие аналоговой синхронизации
        ANALOG_SYNCHRO_ON_RISING_CROSSING_E2010, ANALOG_SYNCHRO_ON_FALLING_CROSSING_E2010,    // аналоговая синхронизация по переходу
        ANALOG_SYNCHRO_ON_HIGH_LEVEL_E2010, ANALOG_SYNCHRO_ON_LOW_LEVEL_E2010,        // аналоговая синхронизация по уровню
        INVALID_ANALOG_SYNCHRO_E2010
    };
}
