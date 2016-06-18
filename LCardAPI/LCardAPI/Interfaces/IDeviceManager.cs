using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCard.Core.Poco;

namespace LCard.API.Interfaces
{
    public interface IDeviceManager
    {
        IE2010 mE2010 { get; set; }
        void StartDetectionLoop();
        void StopDetectionLoop();
        List<SensorPoco> GetAllSensorsFromConfig();
        void GetAllLCardSensors();
        SensorPoco[] Sensors { get; set; }

        bool IsBlockAdapter { get; set; }
    }
}
