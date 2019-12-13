namespace whateverthefuck.src.util
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class Logging
    {
        private static LoggingLevel defaultLevel = LoggingLevel.All;
        private static List<LoggingOutput> outputs = new List<LoggingOutput>();

        public enum LoggingLevel
        {
            All,
            Info,
            Warning,
            Error,
            Fatal,
        }

        public static void SetDefaultLoggingLevel(LoggingLevel l)
        {
            defaultLevel = l;
        }

        public static void AddLoggingOutput(LoggingOutput loutput)
        {
            outputs.Add(loutput);
        }

        public static void Log(object o)
        {
            Log(o.ToString());
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

    public abstract class LoggingOutput
    {
        protected LoggingOutput(Logging.LoggingLevel loggingLevel, bool enabled)
        {
            this.LoggingLevel = loggingLevel;
            this.Enabled = enabled;
        }

        protected Logging.LoggingLevel LoggingLevel { get; set; }

        protected bool Enabled { get; set; }

        public abstract void Write(string message, Logging.LoggingLevel loggingLevel);

        protected bool DoWrite(Logging.LoggingLevel l) => this.Enabled && l >= this.LoggingLevel;
    }

    public class ConsoleOutput : LoggingOutput
    {
        public ConsoleOutput(Logging.LoggingLevel loggingLevel, bool enabled)
            : base(loggingLevel, enabled)
        {
        }

        public override void Write(string message, Logging.LoggingLevel loggingLevel)
        {
            if (!this.DoWrite(loggingLevel))
            {
                return;
            }

            Console.WriteLine(loggingLevel + ": " + message);
        }
    }

    public class FileOutput : LoggingOutput
    {
        private string filePath;

        public FileOutput(Logging.LoggingLevel loggingLevel, bool enabled)
            : base(loggingLevel, enabled)
        {
            this.filePath = "log.txt";
        }

        public FileOutput(string filePath)
            : this(Logging.LoggingLevel.All, true)
        {
            this.filePath = filePath;
        }

        public override void Write(string message, Logging.LoggingLevel loggingLevel)
        {
            if (!this.DoWrite(loggingLevel))
            {
                return;
            }

            using (StreamWriter sw = File.AppendText(this.filePath))
            {
                sw.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + "*" + loggingLevel + "*: " + message);
            }
        }
    }
}