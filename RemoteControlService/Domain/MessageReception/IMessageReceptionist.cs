using Domain.Common.Utilities;
using System;

namespace Domain.MessageReception
{
    public interface IMessageReceptionist
    {
        void Start();
        void Stop();
        Func<string, Maybe<string>> MessageProcessor { get; set; }
    }
}
