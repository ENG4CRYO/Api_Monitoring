using Api_Monitoring.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace Api_Monitoring.Data
{

    public class MonitoringDbContext : DbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options) : base(options)
        {
        }

        public DbSet<ApiLog> ApiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApiLog>(entity =>
            {
                entity.HasIndex(l => l.Timestamp);
                entity.HasIndex(l => l.StatusCode);
            });
        }
    }


    public class MonitoringDbContextFactory : IDesignTimeDbContextFactory<MonitoringDbContext>
    {
        public MonitoringDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MonitoringDbContext>();

           
            optionsBuilder.UseNpgsql("Server=(localdb)\\mssqllocaldb;Database=ApiWatchdog_TempDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

            return new MonitoringDbContext(optionsBuilder.Options);
        }
    }
}