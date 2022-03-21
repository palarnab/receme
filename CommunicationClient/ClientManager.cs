using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using LogManager;

namespace Commander.CommunicationClient
{
    public delegate void CommandExecutorDelegate(string input, IList<string> output);
    internal delegate void ClientPollingDelegate();

    public class CommunicationClientManager
    {
        private AbstractCommunicationClient currentClient;
        private static Hashtable clientPollingThreads = new Hashtable();
        private UserAccount account;

        public CommunicationClientManager(AbstractCommunicationClient client)
        {
            currentClient = client;
        }

        public void StartService(CommandExecutorDelegate informdelegate)
        {
            try
            {
                currentClient.CommandExecutor = informdelegate;
                ClientPollingDelegate clientDelegate = new ClientPollingDelegate(currentClient.Start);
                AsyncCallback callback = new AsyncCallback(StopCallback);
                clientPollingThreads.Add(currentClient, clientDelegate.BeginInvoke(callback, null));
            }
            catch (Exception ex)
            {
                Log.LogObject.Write(Severity.Exception, "Exception caught in Accounts.StartService() \r\n {0}", ex);
            }
        }

        public void StopService()
        {
            try
            {
                IAsyncResult clientAsyncResult = (IAsyncResult)clientPollingThreads[currentClient];

                if (!clientAsyncResult.IsCompleted)
                {
                    AbstractCommunicationClient client = (((AsyncResult)clientAsyncResult).AsyncDelegate as Delegate).Target as AbstractCommunicationClient;
                    client.Stop();

                    DateTime start = DateTime.Now;

                    while (!clientAsyncResult.IsCompleted)
                    {
                        if ((DateTime.Now - start).TotalSeconds > 30) // 30 sec polling max; anywayz this is only a fallback
                        {
                            Log.LogObject.Write(Severity.Error, "ClientPollingThread did not stop and was killed forcefully");
                            break;
                        }

                        Thread.Sleep(1000);
                    }

                    clientPollingThreads.Remove(currentClient);
                }
            }
            catch (Exception ex)
            {
                Log.LogObject.Write(Severity.Exception, "Exception caught in Accounts.StopService() \r\n {0}", ex);
            }
        }

        private void StopCallback(IAsyncResult ar)
        {
            ClientPollingDelegate clientDelegate = (ClientPollingDelegate)((AsyncResult)ar).AsyncDelegate;
            clientDelegate.EndInvoke(ar);
        }
    }
}
