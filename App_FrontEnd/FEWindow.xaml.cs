using System;
using System.IO;
using System.Windows;
using System.Xml;
using Command.Core;
using Commander.CommunicationClient;
using LogManager;

namespace RECEMEapp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FEWindow : Window
    {
        private int pollingDelay = 30;
        private CommandManager commandManager;
        private CommunicationClientManager clientManager;
        private ClientCreator clientCreator;
        private UserAccount account;

        public FEWindow()
        {
            InitializeComponent();

            clientCreator = new ClientCreator();
            commandManager = new CommandManager();
            account = new UserAccount();

            PopulateServices();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!account.UserID.Equals(textBox1.Text) || !account.Password.Equals(textBox2.Password))
            {
                account = new UserAccount(textBox1.Text, textBox2.Password);
            }

            try
            {
                clientManager = new CommunicationClientManager(clientCreator.GetClient(comboBox1.SelectedItem.ToString(), account, pollingDelay));

                clientManager.StartService(new CommandExecutorDelegate(commandManager.Inform));

                EnableDisableControls(false);
                Log.LogObject.Write(Severity.Message, "RECEME started with client - {0}", comboBox1.SelectedItem);
            }
            catch (Exception ex)
            {               
                Log.LogObject.Write(Severity.Exception, "Exception caught in FrontEnd while creating client object \r\n {0}", ex);
                MessageBox.Show("Client initialization failed. \r\nPerhaps UserID/Password was not correct. \r\n\r\nSee log for more information", "Initialization failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            clientManager.StopService();
            EnableDisableControls(true);
            
            Log.LogObject.Write(Severity.Message, "RECEME stopped successfully");
            Log.LogObject.Dispose();
        }

        private void PopulateServices()
        {
            string clientsConfig = Path.Combine(Environment.CurrentDirectory, @"Resource\CommunicationClient.Factory.xml");

            if (File.Exists(clientsConfig))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(clientsConfig);

                foreach (XmlNode node in xml.ChildNodes[1].ChildNodes)
                {
                    comboBox1.Items.Add(node.Attributes["Name"].Value);
                }

                comboBox1.SelectedIndex = 0;
            }
        }

        private void EnableDisableControls(bool status)
        {
            textBox1.IsEnabled = status;
            textBox2.IsEnabled = status;
            comboBox1.IsEnabled = status;
            button1.IsEnabled = status;
            button2.IsEnabled = !status;

            label5.Content = status ? string.Empty : "Service is on air";
        }

        private void comboBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (account.Initialize(comboBox1.SelectedItem))
            {
                textBox1.Text = account.UserID;
                textBox2.Password = account.Password;
            }
        }
    }
}
