using System;
using System.Text;
using Command.Core;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace BasicCommands
{
    public class ProcessInfoCommand : ICommand
    {
        private string name;

        public ProcessInfoCommand()
        {
            name = "processinfo";
        }

        public string Run(string options)
        {
            List<string> processes = new List<string>();
            processes.Add("ID : Name : Responding");

            foreach (Process winProc in Process.GetProcesses())
            {
                if (winProc.ProcessName.StartsWith(options, StringComparison.OrdinalIgnoreCase))
                {
                    processes.Add(string.Format("{0} : {1} : {2}", winProc.Id, winProc.ProcessName, winProc.Responding));
                }
            }

            string path = Path.Combine(Environment.CurrentDirectory + "\\RequestedData", Guid.NewGuid().ToString() + ".txt");
            File.WriteAllLines(path, processes.ToArray());

            return path;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
