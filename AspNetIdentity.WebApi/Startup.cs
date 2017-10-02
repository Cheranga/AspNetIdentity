using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using AspNetIdentity.WebApi.Infrastructure;
using Microsoft.Owin.Cors;
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
            // The ApplicationDbContext and the ApplicationUserManager instances will have a lifetime per request
            //
            builder.CreatePerOwinContext(ApplicationDbContext.Create);
            builder.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
        }
    }
}