namespace Business.Core.Entities
{
    public class ErrorCode
    {
        public int ErrorCodeId { get; set; }
        public string Code { get; set; }
        public string DisplayMessage { get; set; }
        public string Description { get; set; }
    }
}
