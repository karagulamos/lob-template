using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Business.Core.Messaging.Common.Enums;
using Business.Core.Messaging.Common.Helpers;
using Business.Core.Messaging.Entities.Proxies;
using Business.Core.Messaging.Persistence;
using Business.Core.Messaging.Persistence.Repository;
using Business.Core.Messaging.Tasks.Config;
using log4net;
using Quartz;

namespace Business.Core.Messaging.Tasks
{
    internal class EmailSender : ITimedJob
    {
        private static readonly AutoResetEvent ResetEvent = new AutoResetEvent(true);

        private readonly IOutboundEmailRepository _repository;
        private readonly IOutboundEmailConfigurator _emailConfigurator;

        private readonly ILog _logger = LogManager.GetLogger(typeof(EmailSender));

        public EmailSender() : this(DataFactory.Get<IOutboundEmailRepository>(), new OutboundEmailConfigurator())
        { }

        public EmailSender(IOutboundEmailRepository repository, IOutboundEmailConfigurator emailConfigurator)
        {
            _repository = repository;
            _emailConfigurator = emailConfigurator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                ResetEvent.WaitOne();

                try
                {
                    var emails = await _repository.GetOutboundEmailDetailsAsync();

                    var emailCount = emails.Count;

                    if (emailCount <= 0)
                        return;

                    _logger.Info(emailCount + " emails retrieved");

                    var options = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    };

                    Parallel.ForEach(emails, options, email =>
                    {
                        var smptClient = _emailConfigurator.ConfigureSmtpClient(email);
                        var sendStatus = SendEmail(email, smptClient);
                        UpdateEmail(email, sendStatus);
                    });
                }
                finally
                {
                    await _repository.CommitChangesAsync();
                }

                _logger.Info("Processing Emails Complete");
            }
            catch (Exception ex)
            {
                _logger.Info("Exception occurred Processing Emails : " + ex);
            }
            finally
            {
                ResetEvent.Set();
            }
        }

        private void UpdateEmail(OutboundSmtpProxy email, bool sendSuccessful)
        {
            _logger.Debug("Updating " + email.ToLongString());

            try
            {
                //_repository.Reattach(email.OutboundEmail);

                email.OutboundEmail.DateSent = DateTime.Now;
                email.OutboundEmail.Attempts++;
                email.OutboundEmail.EmailStatus = sendSuccessful ? EmailStatus.Sent : EmailStatus.Failed;

                _logger.Info("Update Complete for " + email.ToLongString());
            }
            catch (DbEntityValidationException dbex)
            {
                _logger.Error("Validation Exception occurred while updating  " + email.ToLongString() + " : " + dbex.ToLogString());
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occurred while updating  " + email.ToLongString() + " : " + ex.Message);
            }
        }

        private bool SendEmail(OutboundSmtpProxy email, SmtpClient smtpClient)
        {
            _logger.Info("Sending " + email.ToLongString());

            try
            {
                using (var mail = CreateMailMessage(email))
                {
                    smtpClient.Send(mail);
                }

                _logger.Info("Sending Complete for " + email.ToLongString());

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occurred sending " + email.ToLongString() + " : " + ex.Message);
            }

            _logger.Info("Sending Failed for " + email.ToLongString());

            return false;
        }

        private MailMessage CreateMailMessage(OutboundSmtpProxy detail)
        {
            var email = detail.OutboundEmail;

            var message = new MailMessage
            {
                Subject = email.Subject,
                IsBodyHtml = email.IsHtml
            };

            foreach (var recipient in email.OutboundRecipients)
            {
                message.To.Add(recipient.RecipientEmail);
            }

            foreach (var dbAttachment in email.OutboundAttachments)
            {
                var attachment = new Attachment(dbAttachment.FilePath);
                message.Attachments.Add(attachment);
            }

            var view = AlternateView.CreateAlternateViewFromString(email.Body, null, "text/html");

            if (email.OutboundImages != null && email.OutboundImages.Count > 0)
            {
                foreach (var image in email.OutboundImages)
                {
                    var inlineLogo = new LinkedResource(image.FilePath)
                    {
                        ContentId = Path.GetFileNameWithoutExtension(image.FilePath)
                    };

                    view.LinkedResources.Add(inlineLogo);
                }
            }

            message.AlternateViews.Add(view);
            var from = _emailConfigurator.ConfigureMailAddress(detail);
            message.From = from;
            return message;
        }

        public TimeSpan Interval => TimeSpan.FromSeconds(2);
    }
}
