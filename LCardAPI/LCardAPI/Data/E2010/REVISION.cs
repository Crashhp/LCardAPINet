using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // доступные индексы ревизий модуля E20-10
    public enum REVISION : ushort
    {
        REVISION_A_E2010, 
        REVISION_B_E2010, 
        INVALID_REVISION_E2010
    };
}
