using System;
using System.Diagnostics;
using System.IO;

namespace Ayalon.MFA.CredProvider
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

                //Console.WriteLine(log);
                File.AppendAllText(filePath, log + Environment.NewLine);
            }
        }

        private static string GetFilePath()
        {
            if (path == null)
            {
                var folder = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\Logs\\Ayalon MFA";

                try
                {
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                }
                catch (ArgumentNullException ex)
                {
                    throw new ArgumentNullException(ex.Message);
                }

                catch (DirectoryNotFoundException ex)
                {
                    throw new DirectoryNotFoundException(ex.Message);
                }

                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedAccessException(ex.Message);
                }

                path = Path.Combine(folder, "AyalonMFALog.txt");
            }

            return path;
        }
    }
}
