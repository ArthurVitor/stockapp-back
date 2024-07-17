using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockApp.API.Migrations
{
    /// <inheritdoc />
    public partial class manyToManyExitNoteBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Products_ProductId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryNotes_Inventories_InventoryId",
                table: "EntryNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryNotes_Products_ProductId",
                table: "EntryNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_ExitNotes_Inventories_InventoryId",
                table: "ExitNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_ExitNotes_Products_ProductId",
                table: "ExitNotes");

            migrationBuilder.DropTable(
                name: "BatchExitNote");

            migrationBuilder.CreateTable(
                name: "ExitNoteBatches",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "integer", nullable: false),
                    ExitNoteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitNoteBatches", x => new { x.ExitNoteId, x.BatchId });
                    table.ForeignKey(
                        name: "FK_ExitNoteBatches_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExitNoteBatches_ExitNotes_ExitNoteId",
                        column: x => x.ExitNoteId,
                        principalTable: "ExitNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExitNoteBatches_BatchId",
                table: "ExitNoteBatches",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Products_ProductId",
                table: "Batches",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryNotes_Inventories_InventoryId",
                table: "EntryNotes",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryNotes_Products_ProductId",
                table: "EntryNotes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExitNotes_Inventories_InventoryId",
                table: "ExitNotes",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExitNotes_Products_ProductId",
                table: "ExitNotes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Products_ProductId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryNotes_Inventories_InventoryId",
                table: "EntryNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryNotes_Products_ProductId",
                table: "EntryNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_ExitNotes_Inventories_InventoryId",
                table: "ExitNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_ExitNotes_Products_ProductId",
                table: "ExitNotes");

            migrationBuilder.DropTable(
                name: "ExitNoteBatches");

            migrationBuilder.CreateTable(
                name: "BatchExitNote",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "integer", nullable: false),
                    ExitNoteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchExitNote", x => new { x.BatchId, x.ExitNoteId });
                    table.ForeignKey(
                        name: "FK_BatchExitNote_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchExitNote_ExitNotes_ExitNoteId",
                        column: x => x.ExitNoteId,
                        principalTable: "ExitNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchExitNote_ExitNoteId",
                table: "BatchExitNote",
                column: "ExitNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Products_ProductId",
                table: "Batches",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryNotes_Inventories_InventoryId",
                table: "EntryNotes",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryNotes_Products_ProductId",
                table: "EntryNotes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExitNotes_Inventories_InventoryId",
                table: "ExitNotes",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExitNotes_Products_ProductId",
                table: "ExitNotes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
