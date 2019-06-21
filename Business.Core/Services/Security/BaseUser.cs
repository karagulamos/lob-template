using Business.Core.Common.Enums;

namespace Business.Core.Services.Security
{
    public abstract class BaseUser<TKey>
    {
        public TKey Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public abstract UserType UserType { get; set; }
    }
}