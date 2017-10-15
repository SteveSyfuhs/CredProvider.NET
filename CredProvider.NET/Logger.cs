using System;
using System.Diagnostics;
using System.IO;

namespace CredProvider.NET
{
    internal static class Logger
    {
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

            Console.WriteLine(log);
        }
    }
}
