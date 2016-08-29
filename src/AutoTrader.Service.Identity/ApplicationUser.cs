using System;
using AutoTrader.DomainModel;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoTrader.Service.Identity
{
    public class ApplicationUser : User, IUser<int>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserIdentityManagerService manager)
        {
            throw new NotImplementedException("GenerateUserIdentityAsync");

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
