using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using IRomote;


namespace remoteServer
{
    class Server : MarshalByRefObject, IRomote.IRemoteInterface
    {
        bool messageServer_isChanged = false;             //flag to decide whether the server should broadcast the message to all the clients
        string messageServer="";                          //Broadcast message to be sent to all the clients
        int sendCount = 0;                                //number of clients who has received the broadcast message
        List<string> userList = new List<string>();       //List of user that is connected to server
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(12345);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Server), "Server", WellKnownObjectMode.Singleton);
            Console.WriteLine("Le serveur est bien démarré");
            Console.ReadLine();
           
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }


        /*
        Each client call this function all the time to receive the broadcast message of the server.
        To reduce the traffic in the network, this function is set to be blocking. Which means this 
        fucntion is unblocked only when there is a message to broadcast to all the clents :
        1. A user is connected.
        2. A user is disconnected.
        3. A user has sent a message.
        Each message is sent to all the clients according to the messageServer_isChanged flag and the 
        sendCount variable.
        */
        public string ServerBroadcastMessage()
        {
            while(messageServer_isChanged == false)
            {
                Thread.Sleep(100);
            }
            if(messageServer.Contains("userlist"))
            {
                sendCount++;
                if (sendCount == userList.Count+1)
                {
                    messageServer_isChanged = false;
                    sendCount = 0;
                }

                return messageServer;
            }
            else
            {
                sendCount++;
                if (sendCount == userList.Count)
                {
                    messageServer_isChanged = false;
                    sendCount = 0;
                }

                return messageServer;
            }
            
        }

        //function to update the user list that is connected to the server
        public string UpdateUserlist()
        {
            string users = "userlist|";
            foreach (string userName in userList)
            {
                users = users + userName + "|";
            }
            return users;
        }


        /*
        Function to treat the 3 types of message that client send to server:  
        1. message
        2. login
        3. logout
        */
        public void SendMessage(string message)
        {
            string[] mess_split = message.Split('|');
            string type = mess_split[0];
            string user = mess_split[1];
    
            if(type=="message")
            {
                string content = mess_split[2];
                messageServer = type + "|" + user + "|" + content;
                messageServer_isChanged = true;

            }
            if (type == "login")
            {
                userList.Add(user);
                messageServer = UpdateUserlist();
                messageServer_isChanged = true;

            }
            if (type == "logout")
            {
                userList.Remove(user);
                if(userList.Count>0)
                {
                    messageServer = UpdateUserlist();
                    messageServer_isChanged = true;
                }
                
            }

        }



    }
}

