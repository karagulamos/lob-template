using System.Net.Mail;
using Business.Core.Messaging.Entities.Proxies;

namespace Business.Core.Messaging.Tasks.Config
{
    internal interface IOutboundEmailConfigurator
    {
        SmtpClient ConfigureSmtpClient(OutboundSmtpProxy email);
        MailAddress ConfigureMailAddress(OutboundSmtpProxy email);
    }
}