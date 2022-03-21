using System;
using Command.Core;
using System.Diagnostics;
using System.IO;

namespace BasicCommands
{
    public class NotepadCommand : ICommand
    {
        private string name;

        public NotepadCommand()
        {
            name = "notepad";
        }

        public string Run(string options)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("notepad.exe");
            if (File.Exists(options))
            {
                p.StartInfo.Arguments = options;
            }
            p.Start();
            return null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
