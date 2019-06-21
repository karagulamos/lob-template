using System;

namespace Business.Core.Messaging.Entities
{
    public class OutboundSmtp : ITrackable
    {
        public short OutboundSmtpId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }

        public int DescriptorId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
