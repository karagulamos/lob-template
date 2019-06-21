using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Business.Persistence.Repository.EntityConfiguration
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
