using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoTrader.Service.Identity
{
    public interface IUserIdentityManagerService
    {
        Task<ApplicationUser> FindByIdAsync(int parse);
        Task<ApplicationUser> FindAsync(string email, string password);
        Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType);
        Task<bool> IsInRoleAsync(int userId, string role);
        Task<ApplicationUser> FindByNameAsync(string name);
        Task<IdentityResult> CreateAsync(ApplicationUser identityUser, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser identityUser);
        Task<string> GenerateTwoFactorTokenAsync(int userId, string twoFactorProvider);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(int userId);
        string HashPassword(string newPassword);
        Task<string> GenerateChangePhoneNumberTokenAsync(int userId, string phoneNumber);
        Task<IdentityResult> SetTwoFactorEnabledAsync(int userId, bool enabled);
        Task<IdentityResult> ChangePhoneNumberAsync(int userId, string phoneNumber, string token);
        Task<IdentityResult> SetPhoneNumberAsync(int userId, string phoneNumber);
        Task<string> GetPhoneNumberAsync(int id);
        Task<bool> GetTwoFactorEnabledAsync(int id);
        Task<string> GenerateEmailConfirmationTokenAsync(int userId);
        Task SendEmailAsync(int userId, string subject, string body);
        Task<IdentityResult> ConfirmEmailAsync(int userId, string token);
        Task<IdentityResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(int userId);
        Task<bool> IsEmailConfirmedAsync(int userId);
        Task<IdentityResult> ResetPasswordAsync(int userId, string token, string newPassword);
        Task<IdentityResult> AddToRolesAsync(int userId, params string[] roles);
    }
}
