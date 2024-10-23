using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coffee_PryStore.Migrations
{
    /// <inheritdoc />
    public partial class PhotoADDAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_CartItems_CartItemId",
                table: "Table");

            migrationBuilder.DropIndex(
                name: "IX_Table_CartItemId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Table");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Table",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Table_CartItemId",
                table: "Table",
                column: "CartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_CartItems_CartItemId",
                table: "Table",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id");
        }
    }
}
