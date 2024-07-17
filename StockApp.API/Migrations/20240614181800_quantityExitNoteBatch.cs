using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockApp.API.Migrations
{
    /// <inheritdoc />
    public partial class quantityExitNoteBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "ExitNoteBatches",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ExitNoteBatches");
        }
    }
}
