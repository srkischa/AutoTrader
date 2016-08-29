using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace AutoTrader.Service.Identity
{
    public interface IUserIdentityManagerService
    {
        Task<ApplicationUser> FindAsync(string userName, string password);
        Task<ApplicationUser> FindByNameAsync(string name);
        Task<IdentityResult> CreateAsync(ApplicationUser identityUser, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser identityUser);
    }
}
