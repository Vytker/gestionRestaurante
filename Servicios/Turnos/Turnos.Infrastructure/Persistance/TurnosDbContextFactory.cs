using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Turnos.Infrastructure.Persistence
{
    public class TurnosDbContextFactory
        : IDesignTimeDbContextFactory<TurnosDbContext>
    {
        public TurnosDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Turnos.Api"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var conn = config.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<TurnosDbContext>();
            builder.UseSqlServer(conn);

            return new TurnosDbContext(builder.Options);
        }
    }
}
