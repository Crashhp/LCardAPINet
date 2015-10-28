using System;
using log4net;

namespace LCard.Core.Logger
{
    public class Log4NetLogger : ILogger
    {
        private ILog instance;
        public Log4NetLogger(string logName = "default")
        {
            this.instance = LogManager.GetLogger(logName);
            log4net.Config.XmlConfigurator.Configure();
        }

        public void Info(string message)
        {
            this.instance.Info(message);
            Console.WriteLine(message);
        }

        public void Warning(string message, Exception e = null)
        {
            this.instance.Warn(message, e);
            Console.WriteLine(message);
        }

        public void Error(string message, Exception e = null)
        {
            this.instance.Error(message, e);
            Console.WriteLine(message);
        }

        public void ErrorFormat(string format, object arg0)
        {
            instance.ErrorFormat(format, arg0);
            Console.WriteLine(format, arg0);
        }
    }
}
