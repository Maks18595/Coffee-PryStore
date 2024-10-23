using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coffee_PryStore.Migrations
{
    /// <inheritdoc />
    public partial class CartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Table",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_CartItems_CartItemId",
                table: "Table");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_Table_CartItemId",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Table");
        }
    }
}
