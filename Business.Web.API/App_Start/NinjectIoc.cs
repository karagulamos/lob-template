using System;
using System.Reflection;
using Business.Core.Messaging;
using Business.Core.Persistence;
using Business.Core.Persistence.Cache;
using Business.Core.Persistence.Repository;
using Business.Core.Services;
using Business.Persistence.Cache;
using Business.Persistence.Repository;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Syntax;
using Ninject.Web.Common;

namespace Business.Web.API
{
    public static class NinjectIoc
    {
        public static Lazy<IKernel> CreateKernel = new Lazy<IKernel>(() =>
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            kernel.Bind<ICacheManager>().To<MemoryCacheManager>().InSingletonScope();
            kernel.Bind<BusinessDataContext>().ToSelf().InRequestScope();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork<BusinessDataContext>>();

            kernel.Bind<IMessagingFactory>().To<MessagingFactory>();

            RegisterRepositoriesByConvention(kernel);
            RegisterServicesByConvention(kernel);

            return kernel;
        });

        public static TDependency Get<TDependency>()
        {
            return CreateKernel.Value.Get<TDependency>();
        }

        private static void RegisterRepositoriesByConvention(IBindingRoot root)
        {
            root.Bind(convention => convention
                .FromAssembliesMatching("*")
                .SelectAllClasses()
                .InheritedFrom(typeof(IRepository<>))
                .BindDefaultInterfaces()
            );
        }

        private static void RegisterServicesByConvention(IBindingRoot root)
        {
            root.Bind(convention => convention
                .FromAssembliesMatching("*")
                .SelectAllClasses()
                .InheritedFrom(typeof(IServiceDependencyMarker))
                .BindDefaultInterfaces()
            );
        }
    }
}