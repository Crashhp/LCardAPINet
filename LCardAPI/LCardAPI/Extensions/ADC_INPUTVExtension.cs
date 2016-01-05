using LCard.API.Data.E2010;

namespace LCard.API.Extensions
{
    public static class ADC_INPUTVExtension
    {
        //ADC_INPUTV
        public static ADC_INPUTV Convert(this ADC_INPUTV inputRange, int value)
        {
            switch (value)
            {
                case 0:
                    return ADC_INPUTV.ADC_INPUT_RANGE_3000mV_E2010;
                case 1:
                    return ADC_INPUTV.ADC_INPUT_RANGE_1000mV_E2010;
                case 2:
                    return ADC_INPUTV.ADC_INPUT_RANGE_300mV_E2010;
            }
            return ADC_INPUTV.INVALID_ADC_INPUT_RANGE_E2010;
        }
    }
}
