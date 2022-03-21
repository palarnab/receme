using System;
using Command.Core;
using System.Diagnostics;
using System.IO;

namespace BasicCommands
{
    public class PingCommand : ICommand
    {
        private string name;

        public PingCommand()
        {
            name = "ping";
        }

        public string Run(string options)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("ping.exe");
            p.StartInfo.Arguments = options;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            output += p.StandardOutput.ReadToEnd();
            string path = Path.Combine(Environment.CurrentDirectory + "\\RequestedData", Guid.NewGuid().ToString() + ".txt");
            File.WriteAllText(path, output);

            return path;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
