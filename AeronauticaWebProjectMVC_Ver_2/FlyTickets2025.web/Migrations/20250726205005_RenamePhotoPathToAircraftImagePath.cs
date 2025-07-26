using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlyTickets2025.web.Migrations
{
    /// <inheritdoc />
    public partial class RenamePhotoPathToAircraftImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Aircrafts_AircraftId",
                table: "Seats");

            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "Aircrafts",
                newName: "AircraftImagePath");

            migrationBuilder.AlterColumn<int>(
                name: "AircraftId",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Aircrafts_AircraftId",
                table: "Seats",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Aircrafts_AircraftId",
                table: "Seats");

            migrationBuilder.RenameColumn(
                name: "AircraftImagePath",
                table: "Aircrafts",
                newName: "PhotoPath");

            migrationBuilder.AlterColumn<int>(
                name: "AircraftId",
                table: "Seats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Aircrafts_AircraftId",
                table: "Seats",
                column: "AircraftId",
                principalTable: "Aircrafts",
                principalColumn: "Id");
        }
    }
}
