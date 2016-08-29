using System.Collections.Generic;
using AutoTrader.DomainModel;
using AutoTrader.Data;
using System.Linq;

namespace AutoTrader.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<User> GetUsers()
        {
            return _userRepository.Items.ToList();
        }
    }
}
