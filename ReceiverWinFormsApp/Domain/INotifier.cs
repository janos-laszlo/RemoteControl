using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public interface INotifier
    {
        void Notify(string message);
    }
}
