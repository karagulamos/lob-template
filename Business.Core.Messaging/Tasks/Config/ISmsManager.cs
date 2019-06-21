using System.Collections.Generic;

namespace Business.Core.Messaging.Tasks.Config
{
    public interface ISmsManager
    {
        void Send(string from, string phoneNumber, string body);
        void Send(string from, List<string> phoneNumbers, string body);
    }
}