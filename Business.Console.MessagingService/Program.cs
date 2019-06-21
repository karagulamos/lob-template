using Business.Core.Messaging.Tasks;
using Topshelf;

namespace Business.Console.MessagingService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<MessagingScheduler>(s =>
                {
                    s.ConstructUsing(n => new MessagingScheduler());
                    s.WhenStarted(async service => await service.Start());
                    s.WhenStopped(async service => await service.Stop());
                });

                x.SetDisplayName("Business Messaging Service");
                x.SetDescription("Business Service that Periodically Polls for Outbound Email and Sms Messages");
                x.SetInstanceName("Business_Messaging_Service");
                x.SetServiceName("Business_Messaging_Service");

                x.RunAsLocalSystem();
                // x.UseAssemblyInfoForServiceInfo();
            });
        }
    }
}
