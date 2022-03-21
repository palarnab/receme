using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using LogManager;

namespace Commander.CommunicationClient
{
    public abstract class AbstractCommunicationClient : IEnumerable
    {
        protected readonly string ResponseSubject;
        protected string name;

        protected abstract void SendMail(string from, string to, IList<string> attachments);
        protected abstract void Refresh();

        private bool isActive = false;
        private string userId;
        private int loopWaitTime;
        private IList<string> processedEntries;

        public abstract AbstractCommunicationItem GetCommunicationItem(int index);

        public void Start()
        {
            isActive = true;

            while (isActive)
            {
                Refresh();

                foreach (AbstractCommunicationItem item in this)
                {
                    if (item.IsValid && !IsKnownEntry(item.EntryID))
                    {
                        ProcessCommand(item.From, item.Subject);
                    }

                    RememberEntry(item.EntryID);
                }

                Snooze();
            }

        }

        public void Stop()
        {
            isActive = false; // the only method which is crossthread so far - but has an atomic step so can be left unlocked safely
        }

        public override string ToString()
        {
            return name;
        }

        public IEnumerator GetEnumerator()
        {
            return new CommunicationItemIterator(this);
        }

        public CommandExecutorDelegate CommandExecutor { protected get; set; }

        protected AbstractCommunicationClient(string user, int refreshloop)
        {
            userId = user;
            loopWaitTime = refreshloop;

            AbstractCommunicationItem.ContextIdentifierString = "$CMD";
            ResponseSubject = "Response from RECEME service on " + Environment.MachineName;
            processedEntries = new List<string>();
        }

        private void ProcessCommand(string from, string message)
        {
            try
            {
                IList<string> attachments = new List<string>();
                CommandExecutor(message, attachments);
                TrySendMail(from, attachments);
            }
            catch (Exception ex)
            {
                Log.LogObject.Write(Severity.Exception, "Exception caught in AbstractCommunicationClient.ProcessCommand() \r\n {0}", ex);
            }
        }

        private void TrySendMail(string from, IList<string> attachments)
        {
            int retry = 3;
            while (retry-- > 0)
            {
                try
                {
                    SendMail(userId, from, attachments);
                    Log.LogObject.Write(Severity.Message, "RECEME response sent successfully from client {0}", this);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is NotImplementedException)
                    {
                        throw ex;
                    }
                    Log.LogObject.Write(Severity.Exception, "Exception caught in AbstractCommunicationClient.TrySendMail(). Retry={0}\r\n{1}", retry < 3, ex);
                    Thread.Sleep(5000);
                }
            }
        }

        private void RememberEntry(string entryID)
        {
            if (!IsKnownEntry(entryID))
            {
                processedEntries.Add(entryID);
            }
        }

        private bool IsKnownEntry(string entryID)
        {
            return processedEntries.Contains(entryID);
        }

        private void Snooze()
        {
            DateTime start = DateTime.Now;

            while (isActive)
            {
                if ((DateTime.Now - start).TotalSeconds > loopWaitTime)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }
    }
}
