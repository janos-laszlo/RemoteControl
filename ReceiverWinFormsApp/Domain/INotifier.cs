using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public interface INotifier
    {
        event Action OnActivated;

        void Notify(string message);
    }
}
