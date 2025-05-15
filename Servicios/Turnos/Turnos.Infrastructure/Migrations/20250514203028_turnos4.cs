using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class turnos4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaHoraInicio",
                table: "Turnos",
                newName: "HorarioInicio");

            migrationBuilder.RenameColumn(
                name: "FechaHoraFin",
                table: "Turnos",
                newName: "HorarioFin");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HorarioInicio",
                table: "Turnos",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HorarioFin",
                table: "Turnos",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioFin",
                table: "Slots",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioInicio",
                table: "Slots",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorarioFin",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "HorarioInicio",
                table: "Slots");

            migrationBuilder.RenameColumn(
                name: "HorarioInicio",
                table: "Turnos",
                newName: "FechaHoraInicio");

            migrationBuilder.RenameColumn(
                name: "HorarioFin",
                table: "Turnos",
                newName: "FechaHoraFin");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaHoraInicio",
                table: "Turnos",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaHoraFin",
                table: "Turnos",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }
    }
}
