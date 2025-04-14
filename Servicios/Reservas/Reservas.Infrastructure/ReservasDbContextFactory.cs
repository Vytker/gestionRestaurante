using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Reservas.Infrastructure.Data;

public class ReservasDbContextFactory : IDesignTimeDbContextFactory<ReservasDbContext>
{
    public ReservasDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReservasDbContext>();

        // Usa tu cadena de conexión aquí, por ejemplo:
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=reservadb;User Id=sa;Password=-Vyte123456!;TrustServerCertificate=True;");


        return new ReservasDbContext(optionsBuilder.Options);
    }
}
