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
        void RunDetectionLoop();
        void StopDetectionLoop();
        List<SensorPoco> GetAllSensorsFromConfig();
        void GetAllLCardSensors();
    }
}
