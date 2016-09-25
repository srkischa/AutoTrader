using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace AutoTrader.Service.Identity
{
    public class UserIdentityManagerService : UserManager<ApplicationUser, int>, IUserIdentityManagerService
    {
        public UserIdentityManagerService(
            IUserStore<ApplicationUser, int> store,
            IIdentityMessageService emailService,
            IDataProtectionProvider dataProtectionProvider) : base(store)
        {
            UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("UserToken"));

            EmailService = emailService;

            UserValidator = new UserValidator<ApplicationUser, int>(this)
            {
                AllowOnlyAlphanumericUserNames = false
            };
        }

        public string HashPassword(string newPassword)
        {
            return PasswordHasher.HashPassword(newPassword);
        }
    }
}
