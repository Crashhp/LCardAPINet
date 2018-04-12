using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCardE440
{
    class Program
    {
        public enum AdcInputRange
        {
            ADC_INPUT_RANGE_10000mV_E440,
            ADC_INPUT_RANGE_2500mV_E440,
            ADC_INPUT_RANGE_625mV_E440,
            ADC_INPUT_RANGE_156mV_E440
        }

        static void Main(string[] args)
        {
            E440 card = new E440(0, 30, 10, new int[] { 0 }, new int[] { (int)AdcInputRange.ADC_INPUT_RANGE_10000mV_E440 });
            if (card.Connect() == 0)
            {
                card.ReadData();
                var res = card.GetResult();
            }

            card.Disconnect();
        }
    }
}