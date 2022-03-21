using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Office.Interop.Outlook;
using System.IO;
using Commander.CommunicationClient;

namespace EmailClients
{
    public class OutlookClient : AbstractCommunicationClient
    {
        Application app = null;
        _NameSpace ns = null;
        MAPIFolder inboxFolder = null;
        MAPIFolder subFolder = null;

        public OutlookClient(UserAccount account, int refreshCycle) : base(account.UserID, refreshCycle)
        {
            name = "Outlook";

            app = new Application();
            ns = app.GetNamespace("MAPI");
            ns.Logon(null, null, false, false);
        }

        public override AbstractCommunicationItem GetCommunicationItem(int index)
        {
            EmailItem emailitem = null;

            if (index >= 0 && index < subFolder.Items.Count)
            {
                MailItem item = (MailItem)subFolder.Items[index + 1];
                emailitem = new EmailItem(item.EntryID, item.Subject, item.SenderEmailAddress);
            }

            return emailitem;
        }

        protected override void Refresh()
        {
            inboxFolder = ns.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            subFolder = inboxFolder.Folders["RECEME"]; //folder.Folders[1]; also works
        }

        protected override void SendMail(string from, string to, IList<string> attachments)
        {
            MailItem msg = (MailItem)app.CreateItem(OlItemType.olMailItem);
            Recipient recipient = (Recipient)msg.Recipients.Add(to);
            recipient.Resolve();

            msg.Subject = ResponseSubject;

            foreach (string a in attachments)
            {
                Attachment attach = msg.Attachments.Add(a, (int)OlAttachmentType.olByValue, 1, Path.GetFileName(a));
            }

            msg.Save();
            ((MailItem)msg).Send();
        }
    }
}
