using System;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Creuna.AzureAD.Contracts;
using Creuna.AzureAD.Utils.FeatureToggles;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace Creuna.AzureAD.EpiserverTest
{
    public partial class Startup
    {
        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The Metadata Address is used by the application to retrieve the signing keys used by Azure AD.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        // The Post Logout Redirect Uri is the URL where the user will be redirected after they sign out.
        //
        private static string clientId = ConfigurationManager.AppSettings["Creuna.AzureAD.ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["Creuna.AzureAD.AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["Creuna.AzureAD.Tenant"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["Creuna.AzureAD.PostLogoutRedirectUri"];

        string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        // private static string commonAuthority = String.Format(CultureInfo.InvariantCulture, aadInstance, "common/");
        const string LogoutUrl = "/util/logout.aspx";

        public void ConfigureAuth(IAppBuilder app)
        {
            if (new FeatureToggleDisableAzureAD().FeatureEnabled)
            {
                // feel free to call your own method here
                return;
            }
            else
            {
                app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
                app.UseCookieAuthentication(new CookieAuthenticationOptions());
                app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        RoleClaimType = ClaimTypes.Role
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = context =>
                        {
                            context.HandleResponse();
                            context.Response.Write(context.Exception.Message);
                            return Task.FromResult(0);
                        },
                        RedirectToIdentityProvider = context =>
                        {
                            // Here you can change the return uri based on multisite
                            HandleMultiSitereturnUrl(context);

                            // To avoid a redirect loop to the federation server send 403 
                            // when user is authenticated but does not have access
                            if (context.OwinContext.Response.StatusCode == 401 &&
                                context.OwinContext.Authentication.User.Identity.IsAuthenticated)
                            {
                                context.OwinContext.Response.StatusCode = 403;
                                context.HandleResponse();
                            }
                            return Task.FromResult(0);
                        },
                        SecurityTokenValidated = (ctx) =>
                        {
                            var redirectUri = new Uri(ctx.AuthenticationTicket.Properties.RedirectUri,
                                UriKind.RelativeOrAbsolute);
                            if (redirectUri.IsAbsoluteUri)
                            {
                                ctx.AuthenticationTicket.Properties.RedirectUri = redirectUri.PathAndQuery;
                            }
                            ServiceLocator.Current.GetInstance<IIdentityUpdater>()
                                .UpdateIdentity(ctx.AuthenticationTicket.Identity);
                            //Sync user and the roles to EPiServer in the background
                            ServiceLocator.Current.GetInstance<ISynchronizingUserService>()
                                .SynchronizeAsync(ctx.AuthenticationTicket.Identity);
                            return Task.FromResult(0);
                        }
                    }
                });
                app.UseStageMarker(PipelineStage.Authenticate);
                app.Map(LogoutUrl, map =>
                {
                    map.Run(ctx =>
                    {
                        ctx.Authentication.SignOut();
                        return Task.FromResult(0);
                    });
                });
            }
        }


        private void HandleMultiSitereturnUrl(
                RedirectToIdentityProviderNotification<Microsoft.IdentityModel.Protocols.OpenIdConnectMessage,
                    OpenIdConnectAuthenticationOptions> context)
        {
            // here you change the context.ProtocolMessage.RedirectUri to corresponding siteurl
            // this is a sample of how to change redirecturi in the multi-tenant environment
            if (context.ProtocolMessage.RedirectUri == null)
            {
                var currentUrl = EPiServer.Web.SiteDefinition.Current.SiteUrl;
                context.ProtocolMessage.RedirectUri = new UriBuilder(
                   currentUrl.Scheme,
                   currentUrl.Host,
                   currentUrl.Port,
                   HttpContext.Current.Request.Url.AbsolutePath).ToString();
            }
        }
    }
}
