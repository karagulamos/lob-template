using System.Configuration;
using Business.Core.Messaging.Config.Elements;

namespace Business.Core.Messaging.Config
{
    public class SmtpConfig : ConfigurationSection
    {
        public static SmtpConfig Get()
        {
            return (SmtpConfig)ConfigurationManager.GetSection("smtpConfig");
        }

        [ConfigurationProperty("from", IsDefaultCollection = false)]
        public string From
        {
            get => (string)this["from"];
            set => this["from"] = value;
        }

        [ConfigurationProperty("smtpNode", IsDefaultCollection = false)]
        public SmtpNode SmtpNode
        {
            get => (SmtpNode)this["smtpNode"];
            set => this["smtpNode"] = value;
        }

        [ConfigurationProperty("dbNode", IsDefaultCollection = false)]
        public DbNode DbNode
        {
            get => (DbNode)this["dbNode"];
            set => this["dbNode"] = value;
        }
    }
}
