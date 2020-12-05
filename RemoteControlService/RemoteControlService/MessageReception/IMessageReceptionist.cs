using System;

namespace RemoteControlService.MessageReception
{
    public interface IMessageReceptionist
    {
        void Start();
        void Stop();

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
