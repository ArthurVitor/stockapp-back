using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockApp.API.Migrations
{
    /// <inheritdoc />
    public partial class MovedPerishableFlagFromEntryNoteToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perishable",
                table: "EntryNotes");

            migrationBuilder.AddColumn<bool>(
                name: "Perishable",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perishable",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "Perishable",
                table: "EntryNotes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
