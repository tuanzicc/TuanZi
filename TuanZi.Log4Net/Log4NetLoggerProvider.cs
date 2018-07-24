using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

using Microsoft.Extensions.Logging;



namespace TuanZi.Log4Net
{
    public class Log4NetLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers = new ConcurrentDictionary<string, Log4NetLogger>();
        private const string DefaultLog4NetFileName = "log4net.config";
        private readonly ILoggerRepository _loggerRepository;

        public Log4NetLoggerProvider() : this(DefaultLog4NetFileName)
        { }

        public Log4NetLoggerProvider(string log4NetConfigFile)
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultLog4NetFileName);
            Assembly assembly = Assembly.GetEntryAssembly() ?? GetCallingAssemblyFromStartup();
            _loggerRepository = LogManager.CreateRepository(assembly, typeof(Hierarchy));

            if (File.Exists(file))
            {
                XmlConfigurator.ConfigureAndWatch(_loggerRepository, new FileInfo(file));
                return;
            }
            RollingFileAppender appender = new RollingFileAppender
            {
                Name = "root",
                File = "log\\log_",
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = "yyyyMMdd-HH\".log\"",
                StaticLogFileName = false,
                MaxSizeRollBackups = 10,
                Layout = new PatternLayout("[%d{HH:mm:ss.fff}] %-5p %c T%t %n%m%n")
            };
            appender.ClearFilters();
            appender.AddFilter(new LevelMatchFilter() { LevelToMatch = Level.Debug });
            BasicConfigurator.Configure(_loggerRepository, appender);
            appender.ActivateOptions();
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new Log4NetLogger(_loggerRepository.Name, categoryName);
        }

        private static Assembly GetCallingAssemblyFromStartup()
        {
            var stackTrace = new System.Diagnostics.StackTrace(2);
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                var type = frame.GetMethod()?.DeclaringType;

                if (string.Equals(type?.Name, "Startup", StringComparison.OrdinalIgnoreCase))
                {
                    return type.Assembly;
                }
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            _loggers.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}