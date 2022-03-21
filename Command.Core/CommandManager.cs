using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Reflection;
using LogManager;

namespace Command.Core
{
    public class CommandManager
    {
        private Hashtable commands = new Hashtable();

        public void InitCommands()
        {
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory + "\\commands", "*.dll"))
            {
                Assembly assembly = Assembly.LoadFrom(file);

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetInterface(typeof(ICommand).FullName) != null)
                    {
                        ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
                        object command = ctor.Invoke(new object[] { });
                        string key = command.ToString();
                        if (!commands.Contains(key))
                        {
                            commands.Add(key, command);
                        }
                        else
                        {
                            commands[key] = command;
                        }
                    }
                }
            }
        }

        public void Inform(string message, IList<string> results)
        {
            Request request = Request.Parse(message);

            if (!commands.ContainsKey(request.Command))
            {
                Log.LogObject.Write(Severity.Error, "Command '{0}' was not found. Trying to re-initialize", request.Command);
                InitCommands(); // may be a newly installed command
            }

            RunCommand(request, results);
        }

        private void RunCommand(Request request, IList<string> results)
        {
            if (AdminCommand.ValidateOptions(request))
            {
                ICommand command = commands[request.Command] as ICommand;
                string result = command.Run(request.Options);

                if (!string.IsNullOrEmpty(result))
                {
                    results.Add(result);
                }

                Log.LogObject.Write(Severity.Message, "Command '{0}' was executed and returned {1} attachments", command, results.Count);
            }
            else
            {
                string path = Path.Combine(Environment.CurrentDirectory + "\\RequestedData", Guid.NewGuid().ToString() + ".txt");
                File.WriteAllText(path, string.Format("Command OR SecretKey was not valid"));
                results.Add(path);
            }
        }
    }
}
