using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.util
{
    public abstract class LoggingOutput
    {
        protected LoggingOutput(Logging.LoggingLevel loggingLevel, bool enabled)
        {
            LoggingLevel = loggingLevel;
            Enabled = enabled;
        }

        protected bool DoWrite(Logging.LoggingLevel l) => Enabled && l >= LoggingLevel;

        protected Logging.LoggingLevel LoggingLevel { get; set; }
        protected bool Enabled { get; set; }
        public abstract void Write(string message, Logging.LoggingLevel loggingLevel);
    }

    public class ConsoleOutput : LoggingOutput
    {
        public ConsoleOutput(Logging.LoggingLevel loggingLevel, bool enabled) : base(loggingLevel, enabled)
        {
        }

        public override void Write(string message, Logging.LoggingLevel loggingLevel)
        {
            if (!DoWrite(loggingLevel)) return;

            Console.WriteLine(loggingLevel + ": " + message);
        }
    }

    public class FileOutput : LoggingOutput
    {
        private string filePath;

        public FileOutput(Logging.LoggingLevel loggingLevel, bool enabled) : base(loggingLevel, enabled)
        {
            filePath = "log.txt";
        }

        public FileOutput(string filePath) : this(Logging.LoggingLevel.All, true)
        {
            this.filePath = filePath;
        }

        public override void Write(string message, Logging.LoggingLevel loggingLevel)
        {
            if (!DoWrite(loggingLevel)) return;
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + "*" + loggingLevel + "*: " + message);
            }
        }
    }

    public static class Logging
    {
        public enum LoggingLevel
        {
            All,
            Info,
            Warning,
            Error,
            Fatal,
        }

        private static LoggingLevel defaultLevel = LoggingLevel.All;

        private static List<LoggingOutput> outputs = new List<LoggingOutput>();

        public static void SetDefaultLoggingLevel(LoggingLevel l)
        {
            defaultLevel = l;
        }

        public static void AddLoggingOutput(LoggingOutput loutput)
        {
            outputs.Add(loutput);
        }

        public static void Log(string message)
        {
            Log(message, defaultLevel);
        }

        public static void Log(string message, LoggingLevel l)
        {
            var printedMessage = DateTime.Now.ToString("HH’:’mm’:’ss.fffffff") + ": " + message;

            foreach (var output in outputs)
            {
                output.Write(printedMessage, l);
            }
        }
    }
}