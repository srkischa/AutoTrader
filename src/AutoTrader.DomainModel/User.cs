using System.Collections.Generic;

namespace AutoTrader.DomainModel
{
    public class User : Entity
    {
        public User()
        {
            Roles = new HashSet<Role>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public bool EmailConfirmed { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }

        public string FullName => FirstName + " " + LastName;

        public virtual ICollection<Role> Roles { get; private set; }
    }
}
