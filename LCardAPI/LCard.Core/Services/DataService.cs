using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCard.Core.Interfaces;
using LCard.Core.Poco;

namespace LCard.Core.Services
{
    public class DataService : IDataService
    {
        //Oscilloscope Data File
        //Experiment Time : 2010 Пн май 12 16:14:16
        //Number Of Channels : 4
        //Kadrs Number : 199999
        //Input Rate In kHz: 100.000000
        //Input Time In Sec: 1.999990
        //Decimation: 1
        //Time markers scale: секунды
        //Data as Time Sequence:
        //                    Ch  1      Ch  2      Ch  3      Ch  4  
        public DateTime ExperimentTime { get; set; }
        public int NumberOfChannels { get; set; }
        public int KadrsNumber { get; set; }
        public double InputRateInkHz { get; set; }
        public int Decimation { get; set; }
        public string TimeMarkersScale { get; set; }

        public List<double> Time { get; private set; }
        public List<List<double>> ChDatas { get; private set; }

        public DataService()
        {
            InputRateInkHz = 5000.0;
        }

        public void AddPacket(DataPacketPoco packetPoco)
        {
            for (var i = 0; i < packetPoco.DataSize; i++)
            {
                Time.Add(i/InputRateInkHz);
            }
        }

    }
}
