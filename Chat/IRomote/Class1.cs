using System;

namespace IRomote
{
    public interface IRemoteInterface
    {
        void SendMessage(string message);
        string ServerBroadcastMessage();
    }
}

