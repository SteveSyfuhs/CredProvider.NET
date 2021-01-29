using System;
using System.Diagnostics;
using System.IO;

namespace CredProvider.NET
{
    internal static class Logger
    {
        private static string path;
        private static readonly object signal = new object();

        static Logger()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try
                {
                    Write(e.ExceptionObject.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            };
        }

        public static TextWriter Out
        {
            get { return Console.Out; }
            set { Console.SetOut(value); }
        }

        public static void Write(string line = null, string caller = null)
        {
            if (string.IsNullOrWhiteSpace(caller))
            {
                var method = new StackTrace().GetFrame(1).GetMethod();

                caller = $"{method.DeclaringType?.Name}.{method.Name}";
            }

            var log = $"{DateTimeOffset.UtcNow:u} [{caller}]";

            if (!string.IsNullOrWhiteSpace(line))
            {
                log += " " + line;
            }

            //Just in case multiple threads try to write to the log
            lock (signal)
            {
                var filePath = GetFilePath();

                Console.WriteLine(log);
                File.AppendAllText(filePath, log + Environment.NewLine);
            }
        }

        private static string GetFilePath()
        {
            if (path == null)
            {
                var folder = $"{Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName}\\Logs\\CredProviderNET";

                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                path = $"{folder}\\Log-{DateTime.Now.Ticks}.txt";
            }

            return path;
        }
    }
}
