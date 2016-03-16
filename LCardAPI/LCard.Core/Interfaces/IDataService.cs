using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCard.Core.Poco;

namespace LCard.Core.Interfaces
{
    public interface IDataService
    {
        IList<DataPacketPoco> Packets { get; set; }
        double InputRateInkHz { get; set; }
        DateTime ExperimentTime { get; set; }
        int Decimation { get; set; }
        void AddPacket(DataPacketPoco packetPoco);
        void WriteData(Action<int, int> progressAction, bool[] isChannleEnbled, string path );
    }
}
