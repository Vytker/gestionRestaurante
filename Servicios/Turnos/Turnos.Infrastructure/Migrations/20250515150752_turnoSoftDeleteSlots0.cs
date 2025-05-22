using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class turnoSoftDeleteSlots0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Slots",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Slots");
        }
    }
}
