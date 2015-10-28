using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetReadData.Lusb.Data
{
    // возможные индексы скорости работы модуля на шине USB
    public enum LusbSpeed
    {
        ERROR_USB_SPEED = -1,
        USB11_LUSBAPI = 0,
        USB20_LUSBAPI,
        INVALID_USB_SPEED_LUSBAPI
    };

    // доступные индексы источника сигнала старта сбора данных
    enum INT_ADC_START : ushort
    {
        INT_ADC_START_E2010, INT_ADC_START_WITH_TRANS_E2010,
        EXT_ADC_START_ON_RISING_EDGE_E2010, EXT_ADC_START_ON_FALLING_EDGE_E2010,
        //                EXT_ADC_START_ON_HIGH_LEVEL_E2010, EXT_ADC_START_ON_LOW_LEVEL_E2010,        // для Rev.B и выше (пока нет)
        INVALID_ADC_START_E2010
    };
    // доступные индексы источника тактовых импульсов для запуска АЦП
    enum INT_ADC_CLOCK : ushort { INT_ADC_CLOCK_E2010, INT_ADC_CLOCK_WITH_TRANS_E2010, EXT_ADC_CLOCK_ON_RISING_EDGE_E2010, EXT_ADC_CLOCK_ON_FALLING_EDGE_E2010, INVALID_ADC_CLOCK_E2010 };
    // возможные типы аналоговой синхронизации ввода данных (для Rev.B и выше)
    enum E2010_SYNC : ushort
    {
        NO_ANALOG_SYNCHRO_E2010,            // отсутствие аналоговой синхронизации
        ANALOG_SYNCHRO_ON_RISING_CROSSING_E2010, ANALOG_SYNCHRO_ON_FALLING_CROSSING_E2010,    // аналоговая синхронизация по переходу
        ANALOG_SYNCHRO_ON_HIGH_LEVEL_E2010, ANALOG_SYNCHRO_ON_LOW_LEVEL_E2010,        // аналоговая синхронизация по уровню
        INVALID_ANALOG_SYNCHRO_E2010
    };
    // доступные индексы диапазонов входного напряжения модуля E20-10
    enum ADC_INPUTV : ushort { ADC_INPUT_RANGE_3000mV_E2010, ADC_INPUT_RANGE_1000mV_E2010, ADC_INPUT_RANGE_300mV_E2010, INVALID_ADC_INPUT_RANGE_E2010 };
    // возможные типы подключения входного тракта модуля E20-10
    enum ADC_INPUT : ushort { ADC_INPUT_ZERO_E2010, ADC_INPUT_SIGNAL_E2010, INVALID_ADC_INPUT_E2010 };
    // возможные индексы для управления входным током смещения модуля E20-10 (для Rev.B и выше)
    enum INPUT_CURRENT : ushort { INPUT_CURRENT_OFF_E2010, INPUT_CURRENT_ON_E2010, INVALID_INPUT_CURRENT_E2010 };
    // возможные режимы фиксации факта перегрузки входных каналов при сборе данных (только для Rev.A)
    enum CLIPPING_OVERLOAD : ushort { CLIPPING_OVERLOAD_E2010, MARKER_OVERLOAD_E2010, INVALID_OVERLOAD_E2010 };
    // доступные номера битов ошибок при выполнении сбора данных с АЦП
    enum BUFFER_OVERRUN : ushort
    {
        // битовое поле BufferOverrun структуры DATA_STATE_E2010
        BUFFER_OVERRUN_E2010 = 0x0,        // переполнение внутреннего буфера модуля
        // битовое поле ChannelsOverFlow структуры DATA_STATE_E2010 (для Rev.B и выше)
        OVERFLOW_OF_CHANNEL_1_E2010 = 0x0, OVERFLOW_OF_CHANNEL_2_E2010,    // биты локальных признаков переполнения разрядной сетки соответствующего канала
        OVERFLOW_OF_CHANNEL_3_E2010, OVERFLOW_OF_CHANNEL_4_E2010,            // за время выполнения одного запроса сбора данных ReadData()
        OVERFLOW_E2010 = 0x7                    // бит глобального признака факта переполнения разрядной сетки модуля за всё время сбора данных от момента START_ADC() до STOP_ADC()
    };
    // возможные опции наличия микросхемы ЦАП для модуля E20-10
    enum DAC_INACCESSIBLED : ushort { DAC_INACCESSIBLED_E2010, DAC_ACCESSIBLED_E2010, INVALID_DAC_OPTION_E2010 };
    // доступные индексы ревизий модуля E20-10
    enum REVISION : ushort { REVISION_A_E2010, REVISION_B_E2010, INVALID_REVISION_E2010 };
    // доступные индексы модификиций модуля E20-10
    enum MODIFICATIONS : ushort
    {
        BASE_MODIFICATION_E2010,             // полоса входных частот 1.25 МГц
        F5_MODIFICATION_E2010,                 // полоса входных частот 5.00 МГц
        INVALID_MODIFICATION_E2010
    };

    // доступные битовые константы для задания тестовых режимов работы модуля E20-10
    enum BITCONTSANTS : ushort { NO_TEST_MODE_E2010, TEST_MODE_1_E2010 };

    // константы для работы с модулем
    enum MODULE_CONTSANTS : ushort
    {
        ADC_CHANNELS_QUANTITY_E2010 = 0x4, MAX_CONTROL_TABLE_LENGTH_E2010 = 256,
        ADC_INPUT_RANGES_QUANTITY_E2010 = ADC_INPUTV.INVALID_ADC_INPUT_RANGE_E2010,
        ADC_INPUT_TYPES_QUANTITY_E2010 = ADC_INPUT.INVALID_ADC_INPUT_E2010,
        ADC_CALIBR_COEFS_QUANTITY_E2010 = ADC_CHANNELS_QUANTITY_E2010 * ADC_INPUT_RANGES_QUANTITY_E2010,
        DAC_CHANNELS_QUANTITY_E2010 = 0x2, DAC_CALIBR_COEFS_QUANTITY_E2010 = DAC_CHANNELS_QUANTITY_E2010,
        TTL_LINES_QUANTITY_E2010 = 0x10,        // кол-во входных и выходных цифровых линий
        USER_FLASH_SIZE_E2010 = 0x200,          // размер области пользовательского ППЗУ в байтах
        REVISIONS_QUANTITY_E2010 = REVISION.INVALID_REVISION_E2010,                // кол-во ревизий модуля
        MODIFICATIONS_QUANTITY_E2010 = MODIFICATIONS.INVALID_MODIFICATION_E2010,    // кол-во вариантов исполнения (модификаций) модуля
        ADC_PLUS_OVERLOAD_MARKER_E2010 = 0x5FFF,    // признак 'плюс' перегрузки отсчёта с АЦП (только для Rev.A)
        ADC_MINUS_OVERLOAD_MARKER_E2010 = 0xA000    // признак 'минус' перегрузки отсчёта с АЦП (только для Rev.A)
    };
}