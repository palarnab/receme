using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Yedda;
using Commander.CommunicationClient;

namespace SocialNetworkingClients
{
    public class TwitterClient : AbstractCommunicationClient
    {
        private Twitter objTwitter = null;
        private string pwd;
        private string uid;
        XmlNodeList nodes;

        public TwitterClient(UserAccount account, int refreshCycle) : base(account.UserID, refreshCycle)
        {
            name = "Twitter";
            uid = account.UserID;
            pwd = account.Password;
            objTwitter = new Twitter();
        }

        public override AbstractCommunicationItem GetCommunicationItem(int index)
        {
            ActivityItem item = null;

            if (index >= 0 && index < nodes.Count)
            {
                XmlNode node = nodes[index];
                item = new ActivityItem(node["id"].InnerText, node["text"].InnerText, node["user"]["screen_name"].InnerText);
            }

            return item;
        }

        protected override void Refresh()
        {
            nodes = objTwitter.GetFriendsTimelineAsXML(uid, pwd)["statuses"].ChildNodes;
        }

        protected override void SendMail(string from, string to, IList<string> attachments)
        {
            string updateText = string.Format("@{0}: {1}. {2} attachments saved", to, ResponseSubject, attachments.Count);
            XmlDocument updated = objTwitter.UpdateAsXML(uid, pwd, updateText);
        }
    }
}
