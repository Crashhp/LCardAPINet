using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // доступные индексы модификиций модуля E20-10
    public enum MODIFICATIONS : ushort
    {
        BASE_MODIFICATION_E2010,             // полоса входных частот 1.25 МГц
        F5_MODIFICATION_E2010,                 // полоса входных частот 5.00 МГц
        INVALID_MODIFICATION_E2010
    };
}
