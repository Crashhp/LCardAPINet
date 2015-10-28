using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    // возможные опции наличия микросхемы ЦАП для модуля E20-10
    enum DAC : ushort
    {
        DAC_INACCESSIBLED_E2010, 
        DAC_ACCESSIBLED_E2010, 
        INVALID_DAC_OPTION_E2010
    };
}
