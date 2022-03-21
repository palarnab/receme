using System;
using System.IO;
using System.Net;
using Command.Core;

namespace BasicCommands
{
    public class DeployCommand : ICommand
    {
        private string name;

        public DeployCommand()
        {
            name = "deploy";
        }

        public string Run(string options)
        {
            string destinationpath = Path.Combine(Environment.CurrentDirectory, "Commands");
            destinationpath = Path.Combine(destinationpath, Path.GetFileName(options));

            WebClient client = new WebClient();
            client.DownloadFile(options, destinationpath);
            client.Dispose();

            return null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
