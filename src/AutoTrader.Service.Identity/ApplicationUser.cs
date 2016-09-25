using System;
using AutoTrader.DomainModel;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoTrader.Service.Identity
{
    public class ApplicationUser : User, IUser<int>
    {
        public ApplicationUser()
        {
            //need this;
        }

        public ApplicationUser(string email)
        {
            Email = email;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(IUserIdentityManagerService manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            userIdentity.AddClaim(new Claim(ClaimTypes.Name, FirstName));
            userIdentity.AddClaim(new Claim(ClaimTypes.Surname, LastName));
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, Email));

            return userIdentity;
        }
    }
}
