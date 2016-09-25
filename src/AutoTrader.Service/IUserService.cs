using AutoTrader.DomainModel;
using System.Collections.Generic;

namespace AutoTrader.Service
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User FindByEmail(string email);
    }
}
