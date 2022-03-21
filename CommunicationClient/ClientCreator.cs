using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Commander.CommunicationClient
{
    public class ClientCreator
    {
        private readonly string KnownClients = Path.Combine(Environment.CurrentDirectory, @"Resource\CommunicationClient.Factory.xml");

        public AbstractCommunicationClient GetClient(string clientType, UserAccount account, int refreshDelay)
        {
            AbstractCommunicationClient communicationClient = null;
            string classFullName = string.Empty;
            string assemblypath = string.Empty;

            if (GetAssemblyPath(ref assemblypath, ref classFullName, clientType) && File.Exists(assemblypath))
            {
                Assembly assembly = Assembly.LoadFrom(assemblypath);
                Type type = assembly.GetType(classFullName);

                ConstructorInfo ctor = type.GetConstructor(new Type[2] { typeof(UserAccount), typeof(int) });
                try
                {
                    communicationClient = (AbstractCommunicationClient)ctor.Invoke(new object[] { account, refreshDelay });
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }

            return communicationClient;
        }

        private bool GetAssemblyPath(ref string path, ref string name, string clientType)
        {
            if (File.Exists(KnownClients))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(KnownClients);

                foreach (XmlNode node in xml.ChildNodes[1].ChildNodes)
                {
                    if (node.Attributes["Name"].Value.Equals(clientType, StringComparison.OrdinalIgnoreCase))
                    {
                        string filename = node.Attributes["AssemblyName"].Value;
                        path = node.Attributes["AssemblyPath"].Value;
                        name = node.Attributes["FullClassName"].Value;

                        path = Path.Combine(Environment.CurrentDirectory, path);
                        path = Path.Combine(path, filename);

                        break;
                    }
                }
            }

            return !string.IsNullOrEmpty(path);
        }
    }
}
