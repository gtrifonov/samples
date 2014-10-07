using System.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using System.Configuration;
using System.Web.Http;
using WebApiAzureAD.Api.App_Start;

[assembly: OwinStartup(typeof(WebApiAzureAD.Api.Startup))]
namespace WebApiAzureAD.Api
{

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
 
            ConfigureAuth(app);

            WebApiConfig.Register(config);

            app.UseWebApi(config);
        }

        /// <summary>
        /// Confige Owin Authentication using Azure AD Authentication. Only calls for controllers marked with [System.Web.Http.Authorize] will be authorized
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureAuth(IAppBuilder app)
        {


            AudienceValidator audienceValidator = (audiences, token, parameters) => { return true; };
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {

                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        AudienceValidator = audienceValidator,
                    },
                    Tenant = ConfigurationManager.AppSettings["ida:Tenant"]
                });
        }
    }

}
