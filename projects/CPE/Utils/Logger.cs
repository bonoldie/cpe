using System;
using System.Globalization;
using Microsoft;
using NLog;

namespace CPE.Utils
{
    public class Logger
    {

        private static NLog.Logger NLoggerInstance = LogManager.GetLogger("CPE");

        public static void Setup()
        {
            LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile("logs/CPE-" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture) + ".log");
            });
        }

        public static NLog.Logger NLogger = NLoggerInstance;
    }
}