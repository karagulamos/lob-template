using System;
using System.Collections.Generic;
using Business.Core.Messaging.Persistence.Context;
using Business.Core.Messaging.Persistence.Repository;
using Business.Core.Messaging.Persistence.Repository.Implementation;

namespace Business.Core.Messaging.Persistence
{
    internal class DataFactory
    {
        private static readonly Lazy<IDictionary<Type, Func<BusinessMessagingContext, IRepository>>> LazyRepo;

        static DataFactory()
        {
            var repositories = new Dictionary<Type, Func<BusinessMessagingContext, IRepository>>
            {
                {typeof(IOutboundEmailRepository), context => new OutboundEmailRepository(context) },
                {typeof(IOutboundSmsRepository), context => new OutboundSmsRepository(context) }
            };

            LazyRepo = new Lazy<IDictionary<Type, Func<BusinessMessagingContext, IRepository>>>(() => repositories);
        }

        public static TRepository Get<TRepository>() where TRepository : IRepository
        {
            var type = typeof(TRepository);

            if (!LazyRepo.Value.ContainsKey(type))
                throw new ArgumentException(type.Name);

            return (TRepository)LazyRepo.Value[type].Invoke(new BusinessMessagingContext());
        }
    }
}
