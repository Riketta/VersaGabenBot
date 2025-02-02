using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace VersaGabenBot
{
    public class Logger
    {
        readonly string folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Logs");

        readonly StreamWriter writer = null;
        static Logger logger = null;

        enum MessageType
        {
            Debug,
            Info,
            Warn,
            Error,
        }

        private Logger()
        {
            logger = this;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, $"{Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace}_{DateTime.Now:yyyyMMdd.HHmmss}.txt");
            writer = new StreamWriter(path, true)
            {
                AutoFlush = true
            };
        }

        public static Logger GetLogger()
        {
            return logger ?? new Logger();
        }

        private void Write(string message, MessageType messageType)
        {
            message = $"[{DateTime.Now:o}|{messageType}|{GetCallingClassName()}] {message}";
            writer.WriteLine(message);
            Console.WriteLine(message);
        }

        public void Debug(string message, params object[] args)
        {
            message = string.Format(message, args);
            Write(message, MessageType.Debug);
        }

        public void Info(string message, params object[] args)
        {
            message = string.Format(message, args);
            Write(message, MessageType.Info);
        }

        public void Warn(string message, params object[] args)
        {
            message = string.Format(message, args);
            Write(message, MessageType.Warn);
        }

        public void Error(string message, params object[] args)
        {
            message = string.Format(message, args);
            Write(message, MessageType.Error);
        }

        private static string GetCallingClassName()
        {
            var method = new StackTrace().GetFrame(3).GetMethod();
            var className = method.ReflectedType.Name;

            return className;
        }
    }
}
