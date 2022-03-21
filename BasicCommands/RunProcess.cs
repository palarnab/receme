using System;
using Command.Core;
using System.Diagnostics;
using System.IO;

namespace BasicCommands
{
    public class RunProcessCommand : ICommand
    {
        private string name;

        public RunProcessCommand()
        {
            name = "runprocess";
        }

        public string Run(string options)
        {
            string path = string.Empty;

            string[] commands = options.Split(' ');

            if (commands.Length >= 2)
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(commands[0]);

                for (int i = 1; i < commands.Length; i++)
                {
                    p.StartInfo.Arguments += commands[i] + " ";
                }

                p.StartInfo.Arguments.Trim();
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                output += p.StandardOutput.ReadToEnd();
                path = Path.Combine(Environment.CurrentDirectory + "\\RequestedData", Guid.NewGuid().ToString() + ".txt");
                File.WriteAllText(path, output);
            }

            return path;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
