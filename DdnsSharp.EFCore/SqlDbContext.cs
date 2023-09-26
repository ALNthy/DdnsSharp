using DdnsSharp.Model;
using Microsoft.EntityFrameworkCore;

namespace DdnsSharp.EFCore
{
    public class SqlDbContext : DbContext
    {

        public DbSet<DdnsConfig> DdnsConfigs { get; set; }

        public SqlDbContext(DbContextOptions options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

    }
}