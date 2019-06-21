namespace Business.Core.Messaging.Entities.Proxies
{
    public class AttachmentProxy
    {
        public string FilePath { get; set; }

        public static explicit operator OutboundAttachment(AttachmentProxy attachment)
        {
            return new OutboundAttachment
            {
                FilePath = attachment.FilePath
            };
        }
    }
}
