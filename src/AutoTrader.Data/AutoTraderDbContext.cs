using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AutoTrader.DomainModel;

namespace AutoTrader.Data
{
    public class AutoTraderDbContext : DbContext
    {
        public AutoTraderDbContext() : base("AutoTrader")
        {
            Configuration.AutoDetectChangesEnabled = false;
            Database.Log = s => Debug.WriteLine(s); ;// _log.Debug;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(GetType().Assembly);

            RegisterEntities(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Properties<int>().Where(p => p.Name == "Id")
                .Configure(p => p.HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity));

            modelBuilder.Properties<string>().Configure(p => p.IsRequired().HasMaxLength(255));
            modelBuilder.Properties<decimal>().Configure(p => p.HasPrecision(18, 8));

            base.OnModelCreating(modelBuilder);
        }

        private void RegisterEntities(DbModelBuilder modelBuilder)
        {
            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            var entityTypes = Assembly.GetAssembly(typeof(Entity)).GetTypes()
                .Where(x => x.IsSubclassOf(typeof(Entity)) && !x.IsAbstract);

            foreach (var type in entityTypes)
            {
                entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, new object[] { });
            }
        }
    }
}
