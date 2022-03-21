using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Command.Core
{
    internal class Request
    {
        private const string CommandIdentifier = "command";
        private const string OptionsIdentifier = "options";
        private const string SecretkeyIdentifier = "key";
        private const char ParsingSeparator = ',';
        private const char StartEndSeparator = '"';
        private const char ArgumentSeparator = '=';

        internal string Command { get; private set; }
        internal string Options { get; private set; }
        internal string SecretKey { get; private set; }

        private Request(string command, string options, string secretkey)
        {
            Command = command;
            Options = options;
            SecretKey = secretkey;
        }

        internal static Request Parse(string message)
        {
            string command = null;
            string options = null;
            string key = string.Empty;

            foreach (string parameter in message.Split(ParsingSeparator))
            {
                string arg = parameter.Trim(StartEndSeparator);
                string[] args = arg.Split(ArgumentSeparator);

                if (args.Length == 2)
                {
                    switch (args[0].ToLower())
                    {
                        case CommandIdentifier:
                            command = args[1].Trim(StartEndSeparator).ToLower();
                            break;
                        case OptionsIdentifier:
                            options = args[1].Trim(StartEndSeparator).ToLower();
                            break;
                        case SecretkeyIdentifier:
                            key = args[1].Trim(StartEndSeparator);
                            break;
                    }
                }
            }

            if(string.IsNullOrEmpty(command) || string.IsNullOrEmpty(options))
            {
                throw new ArgumentException(message + " is not a valid Request"); 
            }

            return new Request(command, options, key);
        }
    }
}
