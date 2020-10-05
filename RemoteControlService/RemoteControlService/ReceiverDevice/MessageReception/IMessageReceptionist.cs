using System;

namespace RemoteControlService.ReceiverDevice.MessageReception
{
    public interface IMessageReceptionist
    {
        void Start();
        void Stop();

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
