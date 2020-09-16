using System;

namespace RemoteControlService.ReceiverDevice.MessageReception
{
    interface IMessageReceptionist
    {
        void Start();
        void Stop();

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
