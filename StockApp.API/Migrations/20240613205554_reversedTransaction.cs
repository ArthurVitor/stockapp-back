using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockApp.API.Migrations
{
    /// <inheritdoc />
    public partial class reversedTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reversed",
                table: "Overrides");

            migrationBuilder.AddColumn<bool>(
                name: "Reversed",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reversed",
                table: "Transactions");

            migrationBuilder.AddColumn<bool>(
                name: "Reversed",
                table: "Overrides",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
