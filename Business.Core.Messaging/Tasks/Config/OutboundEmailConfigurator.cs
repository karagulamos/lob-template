using System.Net;
using System.Net.Mail;
using Business.Core.Messaging.Entities.Proxies;

namespace Business.Core.Messaging.Tasks.Config
{
    internal class OutboundEmailConfigurator : IOutboundEmailConfigurator
    {
        public SmtpClient ConfigureSmtpClient(OutboundSmtpProxy email)
        {
            return new SmtpClient(email.OutboundHost)
            {
                Port = email.OutboundPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email.UserName, email.Password),
                EnableSsl = email.UseSsl
            };
        }

        public MailAddress ConfigureMailAddress(OutboundSmtpProxy email)
        {
            return new MailAddress(email.UserName, email.Sender);
        }
    }
}