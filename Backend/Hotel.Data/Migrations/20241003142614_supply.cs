using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Data.Migrations
{
    /// <inheritdoc />
    public partial class supply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Supply_ServiceId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Bookings",
                newName: "SupplyId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ServiceId",
                table: "Bookings",
                newName: "IX_Bookings_SupplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Supply_SupplyId",
                table: "Bookings",
                column: "SupplyId",
                principalTable: "Supply",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Supply_SupplyId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "SupplyId",
                table: "Bookings",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_SupplyId",
                table: "Bookings",
                newName: "IX_Bookings_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Supply_ServiceId",
                table: "Bookings",
                column: "ServiceId",
                principalTable: "Supply",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
