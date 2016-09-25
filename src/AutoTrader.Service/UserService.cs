using System.Collections.Generic;
using AutoTrader.DomainModel;
using AutoTrader.Data;
using System.Linq;

namespace AutoTrader.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public User FindByEmail(string email)
        {
            return _userRepository.Items.FirstOrDefault(user => user.Email == email);
        }

        public IEnumerable<User> GetUsers()
        {
            return _userRepository.Items.ToList();
        }
    }
}
