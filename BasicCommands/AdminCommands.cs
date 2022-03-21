using System;
using Command.Core;
using System.IO;

namespace BasicCommands
{
    public class LockCommand : AdminCommand, ICommand
    {
        private string name;

        public LockCommand()
        {
            name = "lock";
        }

        public string Run(string options)
        {
            string path = Path.Combine(Environment.CurrentDirectory + "\\RequestedData", Guid.NewGuid().ToString() + ".txt");

            if (CreateLock(options))
            {
                File.WriteAllText(path, string.Format("RECEME locked successfully with options={0}", options));
            }
            else
            {
                File.WriteAllText(path, string.Format("RECEME is currently locked"));
            }

            return path;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class UnlockCommand : AdminCommand, ICommand
    {
        private string name;

        public UnlockCommand()
        {
            name = "unlock";
        }

        public string Run(string options)
        {
            string path = Path.Combine(Environment.CurrentDirectory + "\\RequestedData", Guid.NewGuid().ToString() + ".txt");

            if (RemoveLock(options))
            {
                File.WriteAllText(path, string.Format("RECEME unlocked successfully"));
            }
            else
            {
                File.WriteAllText(path, string.Format("RECEME could not be unlocked with options={0}", options));
            }

            return path;
        }


        public override string ToString()
        {
            return name;
        }
    }
}
