using Autofac;
using Autofac.Integration.Owin;
using AutoTrader.Service.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoTrader.WebApi.Provider
{
    public class AutoTraderAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var userManagementService = context.OwinContext.GetAutofacLifetimeScope().Resolve<IUserIdentityManagerService>();

            var user = await userManagementService.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.Rejected();
                context.SetError("Autorization Error", "The username or password is incorrect!");
                context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
            }
            else if (!user.EmailConfirmed)
            {
                context.Rejected();
                context.SetError("Autorization Error", "User did not confirm email.");
                context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
            }
            else
            {
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManagementService, "JWT");
                var ticket = new AuthenticationTicket(oAuthIdentity, null);
                context.Validated(ticket);
            }
        }
    }
}