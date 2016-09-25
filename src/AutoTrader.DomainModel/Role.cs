using System.Collections.Generic;

namespace AutoTrader.DomainModel
{
    public class Role : Entity
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public string Name { get; set; }

        public virtual ICollection<User> Users { get; private set; }
    }
}
