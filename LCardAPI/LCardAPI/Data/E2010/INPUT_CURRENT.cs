using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // возможные индексы для управления входным током смещения модуля E20-10 (для Rev.B и выше)
    public enum INPUT_CURRENT : ushort
    {
        INPUT_CURRENT_OFF_E2010, 
        INPUT_CURRENT_ON_E2010, 
        INVALID_INPUT_CURRENT_E2010
    };
}
