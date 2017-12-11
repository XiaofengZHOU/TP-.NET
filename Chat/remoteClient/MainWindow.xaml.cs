using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using IRomote;
using System.Collections.ObjectModel;
using System.ComponentModel;



namespace remoteClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IRomote.IRemoteInterface remoteService;
        private TcpChannel channel;
        public string UserName { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string MessageContent { get; set; }

        public ObservableCollection<String> UserList { get; set; }
        public ObservableCollection<String> MessageList { get; set; }

        public Boolean isConnected = false;

        Thread t ;
        public MainWindow()
        {
            

            
            UserList = new ObservableCollection<string>();
            MessageList = new ObservableCollection<string>();

            InitializeComponent();
            channel = new TcpChannel();

            // enregistrement du canal
            ChannelServices.RegisterChannel(channel,true);

            // l'ojet LeRemot  récupére ici la référence de l'objet du serveur
            // on donne l'URI (serveur, port, classe du serveur)  et le nom de l'interface
            

            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);

        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if(remoteService!=null && isConnected==true)
            {
                //Your code to handle the event
                string messageType = "logout";
                string message = messageType + "|" + UserName;
                remoteService.SendMessage(message);
            }          
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        /*
         This function is lauched as a thread function when the user is connected to the server.
         This function is used to receive the broadcast message from the server. And accroding 
         to the type message received, it will display the message in the correct place.
        */
        public void Receive_Message()
        {
            while(true)
            {
                string message = remoteService.ServerBroadcastMessage();
                string[] message_split = message.Split('|');
                string type = message_split[0];
                if(type=="userlist")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        UserList.Clear();
                        for(int i=1; i< message_split.Count()-1;i++)
                        {
                            UserList.Add(message_split[i]);
                                      
                        }
                        DataContext = this;
                    });

                }
                if(type=="message")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        string publisher = message_split[1];
                        string content = message_split[2];
                        MessageList.Add(publisher + " : " + content);
                        DataContext = this;
                    });
                   
                }

                Thread.Sleep(100);
            }
        }

        /*
        When a user click the send button, this function will send a message type message to the server.
        */
        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            if(remoteService!=null && isConnected==true)
            {
                MessageContent = ContentBox.Text;
                string messageType = "message";
                string message = messageType + "|" + UserName + "|" + MessageContent;
                remoteService.SendMessage(message);
                ContentBox.Clear();
            }
            
        }

        /*
        When a user click the diconnect button, this function will disconnect to the remote service and send a 
        logout type message to the server.
        */
        private void Button_Click_Disconnect(object sender, RoutedEventArgs e)
        {
            if (remoteService != null && isConnected == true)
            {
                isConnected = false;
                t.Abort();
                string messageType = "logout";
                string message = messageType + "|" + UserName;
                remoteService.SendMessage(message);
                remoteService = null;
                //System.Windows.Application.Current.Shutdown();
            }

        }

        /*
        When a user click the login button, this function will connect to the remote service and send a 
        login type message to the server.
        */
        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            if (isConnected == false)
            {
                isConnected = true;
                remoteService = (IRomote.IRemoteInterface)Activator.GetObject(
                                typeof(IRomote.IRemoteInterface), "tcp://localhost:12345/Server");
                t = new Thread(Receive_Message);
                t.Start();
                UserName = NameBox.Text;
                string messageType = "login";
                string message = messageType + "|" + UserName;
                remoteService.SendMessage(message);
            }
                
        }

    }
}
