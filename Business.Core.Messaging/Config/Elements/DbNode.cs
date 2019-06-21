using System.Configuration;

namespace Business.Core.Messaging.Config.Elements
{
    public class DbNode : ConfigurationElement
    {
        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get => (string)this["connectionString"];
            set => this["connectionString"] = string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
