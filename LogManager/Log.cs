using System;
using System.Text;
using System.IO;

namespace LogManager
{
    public enum Severity
    {
        Info,
        Message,
        Error,
        Exception
    }

    public class Log : IDisposable
    {
        private StreamWriter stream;
        private UTF8Encoding encoding = new System.Text.UTF8Encoding();
        private static bool disposed = true;

        private static Log logobj;

        private Log(string filepath)
        {
            disposed = false;
            stream = File.AppendText(filepath);
            stream.AutoFlush = true;
        }

        public void Write(string message)
        {
            Write(Severity.Info, message);
        }

        public void Write(string message, params object[] parameters)
        {
            Write(Severity.Info, message, parameters);
        }

        public void Write(Severity sev, string message, params object[] parameters)
        {
            stream.WriteLine(string.Format("{0} : {1} : ", DateTime.Now, sev) + string.Format(message, parameters));
        }

        public static Log LogObject
        {
            get
            {
                if (disposed)
                {
                    if (logobj != null)
                    {
                        logobj = null;
                    }
                    logobj = new Log(Path.Combine(Environment.CurrentDirectory + "\\Logs", Guid.NewGuid().ToString() + ".log"));
                }
                return logobj;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // free managed resources
                    stream.Flush();
                    stream.Close();

                    stream.Dispose();

                    GC.SuppressFinalize(this);
                }

                // free native resources if there are any.
                disposed = true;
            }
        }

        ~Log()
        {
            Dispose(false);
        }
    }
}
