using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class turnos5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Assignments",
                newName: "FechaHoraInicio");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraFin",
                table: "Assignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaHoraFin",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "FechaHoraInicio",
                table: "Assignments",
                newName: "Date");
        }
    }
}
