using System.Collections;
using System;

namespace Commander.CommunicationClient
{
    public class EmailItem : AbstractCommunicationItem
    {
        public override string EntryID { get { return entryId; } }
        public override string Subject { get { return subject; } }
        public override string From { get { return from; } }
        public override bool IsValid { get { return isValid; } }

        private string entryId;
        private string subject;
        private string from;
        private bool isValid;

        public EmailItem(string id, string sub, string email)
        {
            entryId = id;
            subject = sub;
            from = email;
            isValid = true;

            if (!string.IsNullOrEmpty(ContextIdentifierString))
            {
                int substring = subject.IndexOf(ContextIdentifierString, StringComparison.OrdinalIgnoreCase);
                isValid = substring != -1;

                if (IsValid)
                {
                    subject = subject.Substring(substring + ContextIdentifierString.Length).Trim();
                }
            }
        }
    }
}
