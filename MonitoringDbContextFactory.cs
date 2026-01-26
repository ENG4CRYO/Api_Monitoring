using Api_Monitoring.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

    public class MonitoringDbContextFactory : IDesignTimeDbContextFactory<MonitoringDbContext>
    {
        public MonitoringDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MonitoringDbContext>();


            optionsBuilder.UseNpgsql("Server=(localdb)\\mssqllocaldb;Database=ApiWatchdog_DesignDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new MonitoringDbContext(optionsBuilder.Options);
        }
    }
