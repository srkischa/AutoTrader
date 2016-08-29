namespace AutoTrader.Data.Migrations.Migrations
{
    using DomainModel;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<AutoTraderDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AutoTraderDbContext context)
        {
            context.Set<User>().AddOrUpdate(user => user.Email,
              new User
              {
                  UserName = "srkischa",
                  FirstName = "Srdjan",
                  LastName = "Milovanov",
                  Email = "s.milovanov@gmail.com",
                  EmailConfirmed = true,
                  PasswordHash = "22",
                  SecurityStamp = "22"
              });

            int result = context.SaveChanges();
        }
    }
}
