using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Business.Core.Messaging.Persistence.Context.Configuration
{
    internal class ContextConfiguration
    {
        [ImportMany(typeof(IEntityConfiguration))]
        public IEnumerable<IEntityConfiguration> Configurations
        {
            get; set;
        }
    }
}
