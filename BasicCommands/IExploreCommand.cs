using System;
using Command.Core;
using System.Diagnostics;
using System.IO;

namespace BasicCommands
{
    public class IExploreCommand : ICommand
    {
        private string name;

        public IExploreCommand()
        {
            name = "iexplore";
        }

        public string Run(string options)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("iexplore.exe");
            p.StartInfo.Arguments = options;
            p.Start();
            return null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
