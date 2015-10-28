using System;

namespace LCard.Core.Logger
{
    public interface ILogger
    {
        void Info(string message);

        void Warning(string message, Exception e = null);

        void Error(string message, Exception e = null);

        void ErrorFormat(string format, object arg0);
    }
}