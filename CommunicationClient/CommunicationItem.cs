using System.Collections;
using System;

namespace Commander.CommunicationClient
{
    public abstract class AbstractCommunicationItem
    {
        public static string ContextIdentifierString { get; set; }

        abstract public string EntryID { get; }
        abstract public string Subject { get; }
        abstract public string From { get; }
        abstract public bool IsValid { get; }
    }

    public class CommunicationItemIterator : IEnumerator
    {
        private AbstractCommunicationClient client;
        private int index = -1;
        private object item = null;

        public CommunicationItemIterator(AbstractCommunicationClient enumerable)
        {
            client = enumerable;
        }

        public object Current
        {
            get
            {
                return item;
            }
        }

        public bool MoveNext()
        {
            index++;

            item = client.GetCommunicationItem(index);
            if (item == null)
            {
                index--;
            }

            return item != null;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
