using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateroom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomNumber",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
