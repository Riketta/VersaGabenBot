using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot
{
    public class Logger
    {
        readonly string folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "logs");

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

            string path = string.Format(Path.Combine(folder, "{0}_{1}.txt"), Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace, DateTime.Now.ToString("yyyyMMdd.HHmmss"));
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
            message = string.Format("[{0}|{1}|{2}] {3}", DateTime.Now.ToString("o"), messageType.ToString(), GetCallingClassName(), message);
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
