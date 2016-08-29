using Autofac;
using Autofac.Integration.Owin;
using AutoTrader.Service.Identity;
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
            else
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                identity.AddClaim(new Claim(ClaimTypes.Email, context.UserName));

                context.Validated(identity);
            }
        }
    }
}