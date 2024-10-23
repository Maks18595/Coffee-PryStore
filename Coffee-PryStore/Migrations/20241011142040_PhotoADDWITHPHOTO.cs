using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coffee_PryStore.Migrations
{
    /// <inheritdoc />
    public partial class PhotoADDWITHPHOTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Table_CategoryTable_CofCateg",
                table: "Table");

            migrationBuilder.DropTable(
                name: "CategoryTable");

            migrationBuilder.DropIndex(
                name: "IX_Table_CofCateg",
                table: "Table");

            migrationBuilder.AlterColumn<string>(
                name: "CofCateg",
                table: "Table",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CofCateg",
                table: "Table",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CategoryTable",
                columns: table => new
                {
                    CategID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategDescript = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTable", x => x.CategID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Table_CofCateg",
                table: "Table",
                column: "CofCateg");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_CategoryTable_CofCateg",
                table: "Table",
                column: "CofCateg",
                principalTable: "CategoryTable",
                principalColumn: "CategID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
