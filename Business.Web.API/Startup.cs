using System;
using System.Web.Http;
using Business.Core.Configuration;
using Business.Web.API;
using Business.Web.API.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
[assembly: OwinStartup(typeof(Startup))]
[assembly: AutoMapperConfig]
namespace Business.Web.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.UseNinjectMiddleware(() => NinjectIoc.CreateKernel.Value)
               .UseNinjectWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            var oAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/login"),
                AllowInsecureHttp = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(8),
                Provider = new BusinessAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
