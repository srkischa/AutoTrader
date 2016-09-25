using AutoTrader.Data;
using AutoTrader.DomainModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTrader.Service.Identity
{
    public class UserStore :
        IUserPasswordStore<ApplicationUser, int>,
        IUserSecurityStampStore<ApplicationUser, int>,
        IUserEmailStore<ApplicationUser, int>,
        IUserRoleStore<ApplicationUser, int>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Role> _roleRepository;

        public UserStore(IRepository<User> userRepository, IRepository<Role> roleRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(ApplicationUser applicationUser)
        {
            if (applicationUser == null) throw new ArgumentNullException(nameof(applicationUser));

            var user = GetUser(applicationUser);

            _userRepository.SaveOrUpdate(user);

            await _unitOfWork.SaveChangesAsync();

            PopulateApplicationUser(applicationUser, user);
        }

        public Task DeleteAsync(ApplicationUser applicationUser)
        {
            if (applicationUser == null) throw new ArgumentNullException(nameof(applicationUser));

            var user = GetUser(applicationUser);

            _userRepository.Delete(user);

            return _unitOfWork.SaveChangesAsync();
        }

        public void Dispose()
        {
            //nothing to dispose
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(int userId)
        {
            var user = _userRepository.Items.FirstOrDefault(x => x.Id == userId);
            return Task.FromResult(GetIdentityUser(user));
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var user = _userRepository.Items.FirstOrDefault(x => x.Email == userName);
            return Task.FromResult(GetIdentityUser(user));
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var u = _userRepository.FindById(user.Id);
            if (u == null)
                throw new ArgumentException("IdentityUser does not correspond to a User entity.", "user");

            PopulateUser(u, user);

            _userRepository.SaveOrUpdate(u);
            return _unitOfWork.SaveChangesAsync();
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
            user.PasswordHash = applicationUser.PasswordHash;
            user.SecurityStamp = applicationUser.SecurityStamp;
            user.FirstName = applicationUser.FirstName;
            user.LastName = applicationUser.LastName;
            user.Email = applicationUser.Email;
            user.EmailConfirmed = applicationUser.EmailConfirmed;
        }

        private ApplicationUser GetIdentityUser(User user)
        {
            if (user == null) return null;

            var identityUser = new ApplicationUser();
            PopulateApplicationUser(identityUser, user);

            return identityUser;
        }

        //todo: maybe to create dto class which will have same properties as user, to unable aU => dto => user, user => dto => au
        //problem here is because this is hard to maintain
        //another solution is some same custom maper http://stackoverflow.com/a/22116996
        private void PopulateApplicationUser(ApplicationUser applicationUser, User user)
        {
            applicationUser.Id = user.Id;
            applicationUser.PasswordHash = user.PasswordHash;
            applicationUser.SecurityStamp = user.SecurityStamp;
            applicationUser.FirstName = user.FirstName;
            applicationUser.LastName = user.LastName;
            applicationUser.UserName = user.Email;
            applicationUser.Email = user.Email;
            applicationUser.EmailConfirmed = user.EmailConfirmed;
        }

        public Task AddToRoleAsync(ApplicationUser applicationUser, string roleName)
        {
            var userAndRole = GetUserAndRole(applicationUser, roleName);

            var user = userAndRole.Item1;
            var role = userAndRole.Item2;

            if (user.Roles.Any(r => r.Id == role.Id))
                throw new Exception("User is already in selected role");

            user.Roles.Add(role);

            return _unitOfWork.SaveChangesAsync(true);
        }

        public Task RemoveFromRoleAsync(ApplicationUser applicationUser, string roleName)
        {
            var userAndRole = GetUserAndRole(applicationUser, roleName);

            var user = userAndRole.Item1;
            var role = userAndRole.Item2;

            if (user.Roles.Any(r => r.Id == role.Id))
            {
                user.Roles.Remove(role);
            }

            return _unitOfWork.SaveChangesAsync(true);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser applicationUser)
        {
            var user = _userRepository.FindById(applicationUser.Id);

            if (user == null)
                throw new ArgumentException("IdentityUser does not correspond to a User entity.", "user");

            var roles = (IList<string>)user.Roles.Select(role => role.Name).ToList();

            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser applicationUser, string roleName)
        {
            var userAndRole = GetUserAndRole(applicationUser, roleName);

            var user = userAndRole.Item1;
            var role = userAndRole.Item2;

            return Task.FromResult(user.Roles.Any(r => r.Id == role.Id));
        }

        private Tuple<User, Role> GetUserAndRole(ApplicationUser applicationUser, string roleName)
        {
            var user = _userRepository.FindById(applicationUser.Id);

            if (user == null)
                throw new ArgumentException("IdentityUser does not correspond to a User entity.", "user");

            var role = _roleRepository.Items.FirstOrDefault(x => x.Name == roleName);

            if (role == null)
                throw new ArgumentException("Role does not exist", "roleName");

            return new Tuple<User, Role>(user, role);
        }
    }
}

