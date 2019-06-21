using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.Core.Messaging.Common.Exceptions;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Entities.Proxies;
using Business.Core.Messaging.Persistence;
using Business.Core.Messaging.Persistence.Repository;
using Business.Core.Messaging.Tasks.Config;

namespace Business.Core.Messaging.Tasks.Managers
{
    internal class EmailManager : IEmailManager
    {
        private readonly IOutboundEmailRepository _repository;

        public EmailManager() : this(DataFactory.Get<IOutboundEmailRepository>())
        {
        }

        public EmailManager(IOutboundEmailRepository repository)
        {
            _repository = repository;
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isHtmlBody, int descriptor)
        {
            await SendAsync(from, to, subject, body, isHtmlBody, "", descriptor);
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isHtmlBody, string fileAttachment, int descriptor)
        {
            await SendAsync(from, new List<string> { to }, subject, body, isHtmlBody, string.IsNullOrEmpty(fileAttachment) ? new List<string>() : new List<string> { fileAttachment }, descriptor);
        }

        public async Task SendAsync(string from, List<string> to, string subject, string body, bool isHtmlBody, List<string> fileAttachments, int descriptor)
        {
            var email = new EmailProxy
            {
                From = from,
                Body = body,
                IsHtml = isHtmlBody,
                Subject = subject,
                DescriptorId = descriptor,
                Recipients = new List<EmailRecipientProxy>(),
                Attachments = new List<AttachmentProxy>()
            };

            foreach (var address in to.Where(address => !string.IsNullOrEmpty(address)))
            {
                email.Recipients.Add(new EmailRecipientProxy
                {
                    EmailAddress = address
                });
            }

            foreach (var attachment in fileAttachments.Where(attachment => !string.IsNullOrEmpty(attachment)))
            {
                email.Attachments.Add(new AttachmentProxy
                {
                    FilePath = attachment
                });
            }

            await SendAsync(email);
        }

        private async Task SendAsync(EmailProxy email)
        {
            if (string.IsNullOrEmpty(email.From))
                throw new EmailFormatException("From field cannot be blank");

            if (email.Recipients.Count <= 0)
                throw new EmailFormatException("A minimum of one recipient is required");

            if (string.IsNullOrEmpty(email.Body))
                throw new EmailFormatException("The body of the mail cannot be blank");

            foreach (var attachment in email.Attachments)
            {
                if (!File.Exists(attachment.FilePath))
                    throw new EmailFormatException("The attached file at " + attachment.FilePath + " does not exist");
            }

            _repository.Add((OutboundEmail)email);

            await _repository.CommitChangesAsync();
        }
    }
}
