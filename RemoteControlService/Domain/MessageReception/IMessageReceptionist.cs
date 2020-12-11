using System;

namespace Domain.MessageReception
{
    public interface IMessageReceptionist
    {
        void Start();
        void Stop();

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
