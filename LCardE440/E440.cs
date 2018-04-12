using LCardE440Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCardE440
{
    public class E440
    {
        private E440Bridge _E440Bridge;

        public bool Connected { get; private set; }
        public int VirtualSlot { get; private set; }
        public double AdcRate { get; private set; }
        public int Duration { get; private set; }
        public int[] InputNumbers { get; private set; }
        public int[] InputRanges { get; private set; }

        public E440(int virtualSlot, double adcRate, int duration, int[] inputNumbers, int[] inputRanges)
        {
            _E440Bridge = new E440Bridge();

            VirtualSlot = virtualSlot;
            AdcRate = adcRate;
            Duration = duration;
            InputNumbers = inputNumbers;
            InputRanges = inputRanges;
        }

        public int Connect()
        {
            int result = 0;
            result = _E440Bridge.InitDevice(AdcRate, Duration, InputNumbers, InputRanges, InputNumbers.Length);
            AdcRate = _E440Bridge.AdcParams.AdcRate;

            if (result == 0)
                Connected = true;

            return result;
        }

        public void Disconnect()
        {
            _E440Bridge.Dispose();
            Connected = false;
        }

        public void ReadData()
        {
            _E440Bridge.ReadData();
        }

        public List<short[]> GetResult()
        {
            List<short[]> result = new List<short[]>();

            short[] allData = _E440Bridge.GetResult();
            int length = allData.Length / InputNumbers.Length;
            short[] tempData = new short[length];

            for (int i = 0; i < InputNumbers.Length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    tempData[j] = allData[j * InputNumbers.Length + i];
                }

                result.Add(tempData.ToArray());
            }

            return result;
        }
    }
}