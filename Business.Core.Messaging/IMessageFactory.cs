using Business.Core.Messaging.Tasks.Config;

namespace Business.Core.Messaging
{
    public interface IMessagingFactory
    {
        IEmailManager GetEmailManager();
        ISmsManager GetSmsManager();
    }
}