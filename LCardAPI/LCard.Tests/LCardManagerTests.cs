using System;
using System.Diagnostics;
using System.Threading;
using LCard.API.Interfaces;
using LCard.API.Modules;
using LCard.Core.Poco;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LCard.Tests
{
    [TestClass]
    public class LCardManagerTests
    {
        [TestMethod]
        public void Test_TTL_OUT()
        {
            IE2010 e2010 = new E2010();
            e2010.StartReadData();
            Thread.Sleep(1000);
            e2010.StopReadData();
            var data = e2010.GetDigitalOut();
        }

        //D10, D9 – 1 канал
        //D12, D11 – 2 канал
        //D14, D13 – 3 канал
        //D16, D15 – 4 канал

        //0 0 – входной сигнал с датчика
        //0 1 – сигнал идентификатора
        //1 0 – сигнал с датчика через делитель 1/2
        //1 1 – питание 

        //4 канал 0 – земля
        //3 канал -2,7 в – -4,963 – кп=1,8
        //2 канал +2,71в – +5,164 – кп=1,9
        //1 канал +2,48 в – +12,234 – кп=4,9

        [TestMethod]
        public void Test_Cycle()
        {
            IE2010 e2010 = new E2010();
            e2010.OnData += delegate (DataPacketPoco poco)
            {
                var data2 = poco.Datas[0, 0];
                Debug.WriteLine("data = " + data2);
            };
            e2010.Init();
            
            e2010.StartReadData();
            Thread.Sleep(20000);
            e2010.StopReadData();
            var data = e2010.GetDigitalOut();
        }
    }
}
