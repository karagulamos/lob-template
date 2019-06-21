using Business.Core.Common.Extensions;
using Microsoft.AspNet.Identity;

namespace Business.Services.Security
{
    public class BusinessPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return password.Encrypt();
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return hashedPassword != HashPassword(providedPassword) 
                ? PasswordVerificationResult.Failed 
                : PasswordVerificationResult.Success;
        }
    }
}