using System;
using System.IO;

namespace Command.Core
{
    public interface ICommand
    {
        string Run(string options);
    }

    public class AdminCommand
    {
        private string AdminCommandPath = Environment.CurrentDirectory + "\\resource\\Admin";

        private const string LOCK_FILE = "locked.lock";
        private const char SPLITTER_CHARACTER = ';';

        protected bool CreateLock(string keyword)
        {
            string path = Path.Combine(AdminCommandPath, LOCK_FILE);
            bool success = false;

            if (!File.Exists(path))
            {
                File.WriteAllText(path, keyword);
                success = true;
            }

            return success;
        }

        protected bool RemoveLock(string keyword)
        {
            string path = Path.Combine(AdminCommandPath, LOCK_FILE);
            bool success = true;

            if (File.Exists(path))
            {
                string locked_keyword = File.ReadAllText(path);
                if (!locked_keyword.Equals(keyword))
                {
                    success = false;
                }
                else
                {
                    File.Delete(path);
                }
            }

            return success;
        }

        internal static bool ValidateOptions(Request request)
        {
            string path = GetLockedFile();

            if (!string.IsNullOrEmpty(path))
            {
                string locked_keyword = File.ReadAllText(path);
                return locked_keyword.Equals(request.SecretKey);
            }

            return true;
        }

        internal static string GetLockedFile()
        {
            string path = Path.Combine(Environment.CurrentDirectory + "\\resource\\Admin", LOCK_FILE);
            if (!File.Exists(path))
            {
                path = string.Empty;
            }

            return path;
        }
    }
}
