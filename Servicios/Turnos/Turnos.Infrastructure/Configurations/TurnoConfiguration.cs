using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnos.Domain.Entities;

namespace Turnos.Infrastructure.Configurations
{
    public class TurnoConfiguration : IEntityTypeConfiguration<Turno>
    {
        public void Configure(EntityTypeBuilder<Turno> builder)
        {
            builder.ToTable("Turnos");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.EmpleadoId).IsRequired();
            builder.Property(t => t.OwnerId).IsRequired();

            builder.HasIndex(t => t.EmpleadoId);
            builder.HasIndex(t => t.OwnerId);

            builder.OwnsOne(t => t.Horario, h =>
            {
                h.Property(v => v.Inicio)
                 .HasColumnName("FechaHoraInicio");
                h.Property(v => v.Fin)
                 .HasColumnName("FechaHoraFin");

                // Índice directamente sobre el valor propio
                h.HasIndex(v => v.Inicio)
                 .HasDatabaseName("IX_Turnos_FechaHoraInicio");
            });
        }
    }
}
