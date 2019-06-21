using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Core.Messaging.Common.Enums;

namespace Business.Core.Messaging.Tasks.Config
{
    public interface IEmailManager
    {
        Task SendAsync(string from, string to, string subject, string body, bool isHtmlBody, int descriptorId = SmtpDescriptor.Default);
        Task SendAsync(string from, string to, string subject, string body, bool isHtmlBody, string fileAttachment, int descriptorId = SmtpDescriptor.Default);
        Task SendAsync(string from, List<string> to, string subject, string body, bool isHtmlBody, List<string> fileAttachments, int descriptorId = SmtpDescriptor.Default);
    }
}
