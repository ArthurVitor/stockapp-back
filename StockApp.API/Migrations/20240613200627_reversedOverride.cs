using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockApp.API.Migrations
{
    /// <inheritdoc />
    public partial class reversedOverride : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Reversed",
                table: "Overrides",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reversed",
                table: "Overrides");
        }
    }
}
