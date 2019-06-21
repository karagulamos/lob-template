using System;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Business.Core.Messaging.Common.Helpers;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Persistence;
using Business.Core.Messaging.Persistence.Repository;
using log4net;
using Quartz;

namespace Business.Core.Messaging.Tasks
{
    internal class SmsSender : ITimedJob
    {
        private static readonly AutoResetEvent ResetEvent = new AutoResetEvent(true);

        public TimeSpan Interval => TimeSpan.FromSeconds(2);

        private readonly IOutboundSmsRepository _repository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(SmsSender));

        private readonly string _apiId = ConfigurationManager.AppSettings["ProviderApiId"];
        private readonly string _username = ConfigurationManager.AppSettings["ProviderUsername"];
        private readonly string _password = ConfigurationManager.AppSettings["ProviderPassword"];
        private readonly string _baseUrl = ConfigurationManager.AppSettings["ProviderBaseUrl"];

        public SmsSender() : this(DataFactory.Get<IOutboundSmsRepository>())
        { }

        public SmsSender(IOutboundSmsRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                ResetEvent.WaitOne();

                try
                {
                    var smsMessages = await _repository.GetUnprocessedSmsesAsync();
                    var smsCount = smsMessages.Count;

                    if (smsCount <= 0)
                        return;

                    _logger.Info(smsCount + " SMS Messages retrieved");

                    var options = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    };

                    Parallel.ForEach(smsMessages, options, smsMessage =>
                    {
                        SendSms(smsMessage);
                        UpdateSms(smsMessage);
                    });
                }
                finally
                {
                    await _repository.CommitChangesAsync();
                }

                _logger.Info("Processing SMS Messages Complete");
            }
            catch (Exception ex)
            {
                _logger.Info("Exception occurred Processing SMS Messages : " + ex);
            }
            finally
            {
                ResetEvent.Set();
            }
        }

        private void UpdateSms(OutboundSms smsMessage)
        {
            _logger.Debug("Updating " + smsMessage.ToLongString());

            try
            {
                smsMessage.DateModified = DateTime.Now;
                smsMessage.Attempts++;

                _logger.Info("Update Complete for " + smsMessage.ToLongString());
            }
            catch (DbEntityValidationException dbex)
            {
                _logger.Info("Validation Exception occurred while updating  " + smsMessage.ToLongString() + " : " + dbex.ToLogString());
            }
            catch (Exception ex)
            {
                _logger.Info("Exception occurred while updating  " + smsMessage.ToLongString() + " : " + ex.Message);
            }
        }

        private void SendSms(OutboundSms smsMessage)
        {
            _logger.Info("Sending " + smsMessage.ToLongString());

            try
            {
                foreach (var recipient in smsMessage.SmsRecipients)
                {
                    var client = new WebClient();
                    // Add a user agent header in case the requested URI contains a query.
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR1.0.3705;)");
                    client.QueryString.Add("user", _username);
                    client.QueryString.Add("password", _password);
                    client.QueryString.Add("api_id", _apiId);

                    var num = recipient.PhoneNumber;

                    client.QueryString.Add("to", num);
                    client.QueryString.Add("text", smsMessage.Message);
                    client.QueryString.Add("from", smsMessage.Sender);

                    using (var data = client.OpenRead(_baseUrl))
                    {
                        if (data != null)
                        {
                            using (var reader = new StreamReader(data))
                            {
                                var response = reader.ReadToEnd();
                                _logger.Debug("Response from sending to " + smsMessage.ToLongString() + " is " + response);
                            }
                        }
                        else
                        {
                            _logger.Debug("Unable to retrieve data from the specified URL: " + _baseUrl);
                        }
                    }
                };

                smsMessage.Sent = 1; // 1 for Success
                smsMessage.DateSent = DateTime.Now;

                _logger.Info("Sending Complete for " + smsMessage.ToLongString());
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occurred sending " + smsMessage.ToLongString() + " : " + ex.Message);
            }
        }
    }
}
