using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoToReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Reservas");
        }
    }
}
