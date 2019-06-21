using System;

namespace Business.Core.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        public string Generate(int length)
        {
            var strippedText = Guid.NewGuid().ToString().Replace("-", string.Empty);
            return strippedText.Substring(0, length);
        }
    }
}
