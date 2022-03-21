using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Commander.CommunicationClient
{
    public class UserAccount
    {
        private readonly string KnownUsers = Path.Combine(Environment.CurrentDirectory, @"Resource\KnownUsers.xml");
        public string UserID { get; private set; }
        public string Password { get; private set; }

        public UserAccount(string userID, string password)
        {
            UserID = userID;
            Password = password;
        }

        public UserAccount()
        {
        }

        public bool Initialize(object client)
        {
            bool success = false;

            if (File.Exists(KnownUsers) && client != null)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(KnownUsers);

                foreach (XmlNode node in xml.ChildNodes[1].ChildNodes)
                {
                    if (node.Attributes["Account"].Value.Equals(client.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        UserID = node.Attributes["Name"].Value;
                        bool isEncrypted = bool.Parse(node.Attributes["Encrypted"].Value);
                        Password = isEncrypted ? DecodeFrom64(node.Attributes["Password"].Value) : node.Attributes["Password"].Value;

                        success = true;
                    }
                }
            }

            return success;
        }

        private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes).Trim('z');

            return returnValue;
        }

        private string DecodeFrom64(string encodedData)
        {
            string returnValue = string.Empty;

            try
            {
                byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
                returnValue = ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            catch (FormatException)
            {
            }

            return returnValue;
        }
    }
}
