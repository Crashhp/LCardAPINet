using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // возможные режимы фиксации факта перегрузки входных каналов при сборе данных (только для Rev.A)
    public enum OVERLOAD : ushort
    {
        CLIPPING_OVERLOAD_E2010, 
        MARKER_OVERLOAD_E2010, 
        INVALID_OVERLOAD_E2010
    };
}
