using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class filterTurnosByEliminado1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_Turnos_RestauranteId_Nombre",
                table: "Turnos");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_RestauranteId_Nombre",
                table: "Turnos",
                columns: new[] { "RestauranteId", "Nombre" },
                unique: true,
                filter: "[Eliminado] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
       name: "IX_Turnos_RestauranteId_Nombre",
       table: "Turnos");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_RestauranteId_Nombre",
                table: "Turnos",
                columns: new[] { "RestauranteId", "Nombre" },
                unique: true);
        }
    }
}
