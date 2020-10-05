using System;

namespace RemoteControlService.ReceiverDevice.MessageReception
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }
}
