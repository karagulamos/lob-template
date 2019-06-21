using Business.Core.Messaging.Tasks.Config;
using Business.Core.Messaging.Tasks.Managers;

namespace Business.Core.Messaging
{
    public class MessagingFactory : IMessagingFactory
    {
        public IEmailManager GetEmailManager()
        {
            return new EmailManager();
        }

        public ISmsManager GetSmsManager()
        {
            return new SmsManager();
        }
    }
}
