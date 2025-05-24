using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class turnoCambioOwnerRestauranteId01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Turnos",
                newName: "RestauranteId");

            migrationBuilder.RenameIndex(
                name: "IX_Turnos_OwnerId",
                table: "Turnos",
                newName: "IX_Turnos_RestauranteId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Slots",
                newName: "RestauranteId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Assignments",
                newName: "RestauranteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RestauranteId",
                table: "Turnos",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Turnos_RestauranteId",
                table: "Turnos",
                newName: "IX_Turnos_OwnerId");

            migrationBuilder.RenameColumn(
                name: "RestauranteId",
                table: "Slots",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "RestauranteId",
                table: "Assignments",
                newName: "OwnerId");
        }
    }
}
