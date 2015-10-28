namespace LCard.Core.Logger
{
    public class Logger
    {
        private static ILogger _commonLogger;

        public static ILogger Current
        {
            get
            {
                return _commonLogger ?? (_commonLogger = new Log4NetLogger());
            }
        }

        public static void SetUp(ILogger l)
        {
            _commonLogger = l;
        }
    }
}
