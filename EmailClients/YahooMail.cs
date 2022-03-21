//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;

//namespace Commander.CommunicationClient
//{
//    public class YahooClient : AbstractCommunicationClient
//    {
//        HttpWebRequest request = null;

//        public YahooClient(Account account, int refreshCycle) : base(account.UserID, refreshCycle)
//        {
//            name = "Yahoo";
//            request = WebRequest.Create("http://mail.yahoo.com") as HttpWebRequest;
//            request.Credentials = new NetworkCredential(account.UserID, account.Password);

//            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
//            {
//                StreamReader reader = new StreamReader(response.GetResponseStream());
//                Console.WriteLine(reader.ReadToEnd());
//            }
//        }

//        public override CommunicationItem GetCommunicationItem(int index)
//        {
//            throw new NotImplementedException("YahooClient GetCommunicationItem(int) not implemented");
//        }

//        protected override void Refresh()
//        {
//        }

//        protected override void SendMail(string from, string to, IList<string> attachments)
//        {
//        }
//    }
//}
