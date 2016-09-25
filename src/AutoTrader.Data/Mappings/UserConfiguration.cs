using AutoTrader.DomainModel;
using System.Data.Entity.ModelConfiguration;

namespace AutoTrader.Data.Mappings
{
    class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            HasMany(user => user.Roles)
                .WithMany(role => role.Users);
        }
    }
}
