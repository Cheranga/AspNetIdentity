using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;

namespace AspNetIdentity.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            var httpConfiguration = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(builder);

            ConfigureOAuthTokenConsumption(builder);

            ConfigureWebApi(httpConfiguration);

            builder.UseCors(CorsOptions.AllowAll);

            builder.UseWebApi(httpConfiguration);
        }

        private void ConfigureWebApi(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();
            
            var jsonFormatter = httpConfiguration.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder builder)
        {
            //
            // The ApplicationDbContext, ApplicationUserManager and ApplicationRoleManager instances will have a lifetime per request
            //
            builder.CreatePerOwinContext(ApplicationDbContext.Create);
            builder.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            builder.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            //
            // Setup OAuth JWT Generation
            //
            var serverOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:51678")
            };
            builder.UseOAuthAuthorizationServer(serverOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder builder)
        {
            var issuer = "http://localhost:51678";
            var audienceId = ConfigurationManager.AppSettings.Get("as:AudienceId");
            var audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings.Get("as:AudienceSecret"));


            builder.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] {audienceId},
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                }
            });

        }
    }
}