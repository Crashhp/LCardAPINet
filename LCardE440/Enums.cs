using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCardE440
{
    public static class EnumExtensions
    {
        public static int ToInt<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be an enumerated type");

            return Convert.ToInt32(value);
        }

        public static ushort ToUShort<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be an enumerated type");

            return Convert.ToUInt16(value);
        }
    }

    /// <summary>
    /// Доступные состояния сброса модуля E14-440
    /// </summary>
    public enum ResetModule
    {
        INIT_E440,
        RESET_E440,
        INVALID_RESET_TYPE_E440
    };

    /// <summary>
    /// Доступные индексы диапазонов входного напряжения модуля E14-440
    /// </summary>
    public enum AdcInputRange
    {
        ADC_INPUT_RANGE_10000mV_E440,
        ADC_INPUT_RANGE_2500mV_E440,
        ADC_INPUT_RANGE_625mV_E440,
        ADC_INPUT_RANGE_156mV_E440,
        INVALID_ADC_INPUT_RANGE_E440
    };

    /// <summary>
    /// Тип подключения для входов АЦП
    /// </summary>
    public enum AdcInputType
    {
        ADC_16_DIFF,
        ADC_32_NOT_DIFF
    }

    /// <summary>
    /// Возможные типы синхронизации модуля E14-440
    /// </summary>
    public enum InputMode
    {
        NO_SYNC_E440,
        TTL_START_SYNC_E440,
        TTL_KADR_SYNC_E440,
        ANALOG_SYNC_E440,
        INVALID_SYNC_E440
    };

    /// <summary>
    /// Доступные индексы ревизий модуля E14-440
    /// </summary>
    public enum Revision
    {
        REVISION_A_E440,
        REVISION_B_E440,
        REVISION_C_E440,
        REVISION_D_E440,
        REVISION_E_E440,
        REVISION_F_E440,
        INVALID_REVISION_E440
    };

    /// <summary>
    /// Возможные опции наличия микросхемы ЦАП
    /// </summary>
    public enum DacActive
    {
        DAC_INACCESSIBLED_E440,
        DAC_ACCESSIBLED_E440,
        INVALID_DAC_OPTION_E440
    };

    /// <summary>
    /// Доступные индексы источника тактовых импульсов для АЦП
    /// </summary>
    public enum AdcClock
    {
        INT_ADC_CLOCK_E440,
        INT_ADC_CLOCK_WITH_TRANS_E440,
        EXT_ADC_CLOCK_E440,
        INVALID_ADC_CLOCK_E440
    };

    /// <summary>
    /// Возможные типы DSP (сейчас только ADSP-2185)
    /// </summary>
    public enum DspType
    {
        ADSP2184_E440,
        ADSP2185_E440,
        ADSP2186_E440,
        INVALID_DSP_TYPE_E440
    };

    /// <summary>
    /// Возможные тактовые частоты модудя (сейчас только 24000 кГц)
    /// </summary>
    public enum ClockFrequency
    {
        F14745_E440,
        F16667_E440,
        F20000_E440,
        F24000_E440,
        INVALID_QUARTZ_FREQ_E440
    };
}
