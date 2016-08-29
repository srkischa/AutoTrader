using Microsoft.AspNet.Identity;

namespace AutoTrader.Service.Identity
{
    public class UserIdentityManagerService : UserManager<ApplicationUser, int>, IUserIdentityManagerService
    {
        public UserIdentityManagerService(IUserStore<ApplicationUser, int> store) : base(store)
        { }

        public string HashPassword(string newPassword)
        {
            return PasswordHasher.HashPassword(newPassword);
        }
    }
}
