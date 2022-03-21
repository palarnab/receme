using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using Commander.CommunicationClient;

namespace EmailClients
{
    public class ExchangeClient : AbstractCommunicationClient
    {
        private ExchangeService service = null;
        private FindItemsResults<Item> findResults = null;

        public ExchangeClient(UserAccount account, int refreshCycle) : base(account.UserID, refreshCycle)
        {
            name = "Exchange";

            service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            //service.Credentials = new NetworkCredential(account.UserID, account.Password, "in002");
            service.AutodiscoverUrl(account.UserID);
            //service.Url = new System.Uri("https://owa.siemens.net/EWS/Services.wsdl");
        }

        public override AbstractCommunicationItem GetCommunicationItem(int index)
        {
            EmailItem emailitem = null;

            if (index >= 0 && index < findResults.Items.Count)
            {
                Item item = findResults.Items[index];
                service.LoadPropertiesForItems(new Item[] { item }, new PropertySet(EmailMessageSchema.Sender));
                string from = (item as EmailMessage).Sender.Address;

                emailitem = new EmailItem(item.Id.ToString(), item.Subject, from);
            }

            return emailitem;
        }

        protected override void Refresh()
        {
            findResults = service.FindItems(WellKnownFolderName.Inbox, new ItemView(10));
        }

        protected override void SendMail(string from, string to, IList<string> attachments)
        {
            EmailMessage message = new EmailMessage(service);
            message.Subject = ResponseSubject;
            message.ToRecipients.Add(to);

            foreach (string a in attachments)
            {
                message.Attachments.AddFileAttachment(a);
            }

            message.SendAndSaveCopy();
        }
    }
}
