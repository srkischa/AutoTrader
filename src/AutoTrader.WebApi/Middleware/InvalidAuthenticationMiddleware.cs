using Microsoft.Owin;
using System.Threading.Tasks;

namespace AutoTrader.WebApi.Middleware
{
    public class InvalidAuthenticationMiddleware : OwinMiddleware
    {
        public InvalidAuthenticationMiddleware(OwinMiddleware next)
            : base(next)
        { }

        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            if (context.Response.StatusCode == 400 &&
                context.Response.Headers.ContainsKey("AuthorizationResponse"))
            {
                context.Response.Headers.Remove("AuthorizationResponse");
            }

            if (context.Response.StatusCode == 401 && context.Authentication.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 403;
            }
        }
    }
}