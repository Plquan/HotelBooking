using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Data.Migrations
{
    /// <inheritdoc />
    public partial class _123321 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Transactions");
        }
    }
}
