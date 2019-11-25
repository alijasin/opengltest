using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Util
{
    public abstract class LoggingOutput
    {
        protected LoggingOutput(Logger.LoggingLevel loggingLevel, bool enabled)
        {
            LoggingLevel = loggingLevel;
            Enabled = enabled;
        }

        protected bool DoWrite(Logger.LoggingLevel l) => Enabled && l >= LoggingLevel;

        protected Logger.LoggingLevel LoggingLevel { get; set; }
        protected bool Enabled { get; set; }
        public abstract void Write(string message, Logger.LoggingLevel loggingLevel);
    }

    public class ConsoleOutput : LoggingOutput
    {
        public ConsoleOutput(Logger.LoggingLevel loggingLevel, bool enabled) : base(loggingLevel, enabled)
        {
        }

        public override void Write(string message, Logger.LoggingLevel loggingLevel)
        {
            if (!DoWrite(loggingLevel)) return;

            Console.WriteLine(loggingLevel + ": " + message);
        }
    }

    public class FileOutput : LoggingOutput
    {
        private string filePath;
        public FileOutput(Logger.LoggingLevel loggingLevel, bool enabled) : base(loggingLevel, enabled)
        {
            filePath = "log.txt";
        }

        public override void Write(string message, Logger.LoggingLevel loggingLevel)
        {
            if (!DoWrite(loggingLevel)) return;
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + "*" + loggingLevel + "*: " + message);
            }
        }
    }

    public static class Logger
    {
        public enum LoggingLevel
        {
            All,
            Info,
            Warning,
            Error,
            Fatal,
        }

        private static LoggingLevel defaultLevel;

        private static List<LoggingOutput> outputs = new List<LoggingOutput>();

        public static void SetDefaultLoggingLevel(LoggingLevel l)
        {
            defaultLevel = l;
        }

        public static void AddLoggingOutput(LoggingOutput loutput)
        {
            outputs.Add(loutput);
        }

        public static void Write(string message)
        {
            Write(message, defaultLevel);
        }

        public static void Write(string message, LoggingLevel l)
        {
            foreach (var output in outputs)
            {
                output.Write(message, l);
            }
        }
    }
}
