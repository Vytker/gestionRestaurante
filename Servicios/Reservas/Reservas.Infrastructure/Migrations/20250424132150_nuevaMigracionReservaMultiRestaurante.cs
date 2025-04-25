using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nuevaMigracionReservaMultiRestaurante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Turnos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "RestauranteId",
                table: "Turnos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestauranteId",
                table: "Reservas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_RestauranteId_Nombre",
                table: "Turnos",
                columns: new[] { "RestauranteId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_RestauranteId_Codigo",
                table: "Reservas",
                columns: new[] { "RestauranteId", "Codigo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turnos_RestauranteId_Nombre",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_RestauranteId_Codigo",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "RestauranteId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "RestauranteId",
                table: "Reservas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
