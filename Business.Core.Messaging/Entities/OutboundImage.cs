
namespace Business.Core.Messaging.Entities
{

    public class OutboundImage
    {
        public long OutboundImageId { get; set; }

        public string FilePath { get; set; }

        public long OutboundEmailId { get; set; }
        public virtual OutboundEmail OutboundEmail { get; set; }
    }
}
