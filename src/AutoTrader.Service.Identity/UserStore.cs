using AutoTrader.Data;
using AutoTrader.DomainModel;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTrader.Service.Identity
{
    public class UserStore : IUserPasswordStore<ApplicationUser, int>,
        IUserSecurityStampStore<ApplicationUser, int>,
        IUserEmailStore<ApplicationUser, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;

        public UserStore(IRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var newUser = GetUser(user);

            _userRepository.SaveOrUpdate(newUser);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _userRepository.Delete(user.Id);
            await _unitOfWork.SaveChangesAsync();
        }

        public void Dispose()
        {
            //nothing to dispose
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var user = await _userRepository.Items.FirstOrDefaultAsync(u => u.Email == email);
            return GetIdentityUser(user);
        }

        public async Task<ApplicationUser> FindByIdAsync(int userId)
        {
            var user = await _userRepository.Items.FirstOrDefaultAsync(u => u.Id == userId);
            return GetIdentityUser(user);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var user = await _userRepository.Items.FirstOrDefaultAsync(u => u.UserName == userName);
            return GetIdentityUser(user);
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var oldUser = _userRepository.FindById(user.Id);

            if (oldUser == null)
            {
                throw new  ArgumentException("User does not exist", nameof(user));
            }

            PopulateUser(oldUser, user);

            _userRepository.SaveOrUpdate(oldUser);
            await _unitOfWork.SaveChangesAsync();
        }

        private User GetUser(ApplicationUser identityUser)
        {
            if (identityUser == null)
                return null;

            var user = new User();
            PopulateUser(user, identityUser);

            return user;
        }

        private void PopulateUser(User user, ApplicationUser applicationUser)
        {
            user.Id = applicationUser.Id;
            user.FirstName = applicationUser.FirstName;
            user.LastName = applicationUser.LastName;
            user.Email = applicationUser.UserName;
            user.SecurityStamp = applicationUser.SecurityStamp;
            user.Email = applicationUser.Email;
            user.EmailConfirmed = applicationUser.EmailConfirmed;
            user.PasswordHash = applicationUser.PasswordHash;
        }

        private ApplicationUser GetIdentityUser(User user)
        {
            if (user == null) return null;

            var identityUser = new ApplicationUser();
            PopulateApplicationUser(identityUser, user);

            return identityUser;
        }

        private void PopulateApplicationUser(ApplicationUser applicationUser, User user)
        {
            applicationUser.Id = user.Id;
            applicationUser.FirstName = user.FirstName;
            applicationUser.LastName = user.LastName;
            applicationUser.UserName = user.Email;
            applicationUser.SecurityStamp = user.SecurityStamp;
            applicationUser.Email = user.Email;
            applicationUser.EmailConfirmed = user.EmailConfirmed;
            applicationUser.PasswordHash = user.PasswordHash;
        }
    }
}
