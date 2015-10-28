using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.API.Data.E2010
{
    public  class MODULE_CONSTANTS
    {
        public const ushort ADC_CHANNELS_QUANTITY_E2010 = 0x4;
        public const ushort MAX_CONTROL_TABLE_LENGTH_E2010 = 256;
        public const ushort ADC_INPUT_RANGES_QUANTITY_E2010 = (ushort)ADC_INPUTV.INVALID_ADC_INPUT_RANGE_E2010;
        public const ushort ADC_INPUT_TYPES_QUANTITY_E2010 = (ushort) ADC_INPUT.INVALID_ADC_INPUT_E2010;

        public const ushort ADC_CALIBR_COEFS_QUANTITY_E2010 = ADC_CHANNELS_QUANTITY_E2010*ADC_INPUT_RANGES_QUANTITY_E2010;

        public const ushort DAC_CHANNELS_QUANTITY_E2010 = 0x2;
        public const ushort DAC_CALIBR_COEFS_QUANTITY_E2010 = DAC_CHANNELS_QUANTITY_E2010;
        public const ushort TTL_LINES_QUANTITY_E2010 = 0x10;        // кол-во входных и выходных цифровых линий
        public const ushort USER_FLASH_SIZE_E2010 = 0x200;          // размер области пользовательского ППЗУ в байтах
        public const ushort REVISIONS_QUANTITY_E2010 = (ushort)REVISION.INVALID_REVISION_E2010;                // кол-во ревизий модуля
        public const ushort MODIFICATIONS_QUANTITY_E2010 = (ushort)MODIFICATIONS.INVALID_MODIFICATION_E2010;    // кол-во вариантов исполнения (модификаций) модуля
        public const ushort ADC_PLUS_OVERLOAD_MARKER_E2010 = 0x5FFF;    // признак 'плюс' перегрузки отсчёта с АЦП (только для Rev.A)

        public const ushort ADC_MINUS_OVERLOAD_MARKER_E2010 = 0xA000; // признак 'минус' перегрузки отсчёта с АЦП (только для Rev.A)
    }
}
