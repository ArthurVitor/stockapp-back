﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockApp.API.Migrations
{
    /// <inheritdoc />
    public partial class Removedperishablefromtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perishable",
                table: "Batches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Perishable",
                table: "Batches",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
