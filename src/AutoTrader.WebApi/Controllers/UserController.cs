using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoTrader.DomainModel;
using AutoTrader.Service;

namespace AutoTrader.WebApi.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            if (userService == null) throw new ArgumentNullException(nameof(userService));
            _userService = userService;
        }

        public IEnumerable<User> Get()
        {
            return _userService.GetUsers();
        }
    }
}
