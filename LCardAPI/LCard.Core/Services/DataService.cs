using System;
using System.Collections.Generic;
using System.IO;
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
        public double InputTimeInSec { get; set; }
        public int Decimation { get; set; }
        public string TimeMarkersScale { get; set; }

        public IList<DataPacketPoco> Packets { get; set; }

        public DataService()
        {
            Packets = new List<DataPacketPoco>(); 
            InputRateInkHz = 100.0;
            ExperimentTime = DateTime.UtcNow;
            Decimation = 1;
            TimeMarkersScale = "секунды";
            NumberOfChannels = 4;
        }

        public void AddPacket(DataPacketPoco packetPoco)
        {
            lock (this)
            {
                Packets.Add(packetPoco);
            }
        }


        public void WriteData()
        {
            lock (this)
            {
                var orderedPackets = Packets.OrderBy(p => p.NumberBlock).ToList();
                NumberOfChannels = orderedPackets.First().NumberOfChannels;
                var fileStream = new StreamWriter("data.txt", false, Encoding.UTF8);
                KadrsNumber = orderedPackets.Count*orderedPackets.First().DataSize;
                var inputRateInHz = InputRateInkHz * 1000;
                InputTimeInSec = KadrsNumber/inputRateInHz;
                fileStream.WriteLine("Oscilloscope Data File");
                fileStream.WriteLine("Experiment Time : 2015 Вт янв 5 15:45:16");
                fileStream.WriteLine("Number Of Channels : {0}", NumberOfChannels);
                fileStream.WriteLine("Kadrs Number : {0}", KadrsNumber);
                fileStream.WriteLine("Input Rate In kHz: {0:0.0000000000}", InputRateInkHz);
                fileStream.WriteLine("Input Time In Sec: {0:0.0000000000}", InputTimeInSec);
                fileStream.WriteLine("Decimation: {0}", Decimation);
                fileStream.WriteLine("Time markers scale: {0}", TimeMarkersScale);
                fileStream.WriteLine("Data as Time Sequence:");
                fileStream.WriteLine("                    Ch  1      Ch  2      Ch  3      Ch  4  ");
                var k = 0;
                var doubleFormat = "0.0000000000";
                var firstPart = "      ";
                var delimeter = "    ";
                StringBuilder builder = new StringBuilder();
                foreach (var dataPacketPoco in orderedPackets)
                {
                    IList<string> linesPacket = new List<string>();
                    for (var i = 0; i < dataPacketPoco.DataSize; i++)
                    {
                        var time = k/inputRateInHz;
                        switch (NumberOfChannels)
                        {
                            case 1:
                                linesPacket.Add($"      {time:0.0000000000}    {dataPacketPoco.Datas[0, i]:0.0000000000}");
                                break;
                            case 2:
                                linesPacket.Add($"      {time:0.0000000000}    {dataPacketPoco.Datas[0, i]:0.0000000000}     {dataPacketPoco.Datas[1, i]:0.0000000000}");
                                break;
                            case 3:
                                linesPacket.Add($"      {time:0.0000000000}    {dataPacketPoco.Datas[0, i]:0.0000000000}     {dataPacketPoco.Datas[1, i]:0.0000000000}     {dataPacketPoco.Datas[2, i]:0.0000000000}");
                                break;
                            case 4:
                                linesPacket.Add(String.Concat(firstPart, time.ToString(doubleFormat), delimeter, dataPacketPoco.Datas[0, i].ToString(doubleFormat), delimeter, dataPacketPoco.Datas[1, i].ToString(doubleFormat), delimeter, dataPacketPoco.Datas[2, i].ToString(doubleFormat), delimeter, dataPacketPoco.Datas[3, i].ToString(doubleFormat)));
                                

                                //linesPacket.Add($"      {time:0.0000000000}    {dataPacketPoco.Datas[0, i]:0.0000000000}    {dataPacketPoco.Datas[1, i]:0.0000000000}    {dataPacketPoco.Datas[2, i]:0.0000000000}    {dataPacketPoco.Datas[3, i]:0.0000000000}");
                                //linesPacket.Add(String.Format("      {0:0.0000000000}    {1:0.0000000000}    {2:0.0000000000}    {3:0.0000000000}    {4:0.0000000000}",time, dataPacketPoco.Datas[0, i], dataPacketPoco.Datas[1, i], dataPacketPoco.Datas[2, i], dataPacketPoco.Datas[3, i]));
                                break;
                                
                        }
                        k++;
                    }
                    foreach (var linePacket in linesPacket)
                    {
                        fileStream.WriteLine(linePacket);
                    }

                }
                fileStream.Flush();
                fileStream.Close();
            }
        }
    }
}
