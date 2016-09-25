using AutoTrader.DomainModel;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace AutoTrader.Service
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private User _currentUser;
        private readonly IPrincipal _principal;
        private readonly IUserService _userService;

        public CurrentUserProvider(IPrincipal principal, IUserService userService)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            if (userService == null)
            {
                throw new ArgumentNullException(nameof(userService));
            }

            _principal = principal;
            _userService = userService;
        }

        public User User
        {
            get
            {
                if (_currentUser == null)
                {
                    var claimsIdentity = _principal.Identity as ClaimsIdentity;

                    if (claimsIdentity != null && claimsIdentity.IsAuthenticated)
                    {
                        var email = claimsIdentity.FindFirst(ClaimTypes.Email);
                        _currentUser = _userService.FindByEmail(email.Value);
                    }
                }

                return _currentUser;
            }
        }
    }
}
