using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.Core.Poco
{
    public class DataPacketPoco
    {
        public float[,] Datas { get; set; }
        public int NumberOfChannels { get; set; }
        public int DataSize { get; set; }
        public int NumberBlock { get; set; }
    }
}
