using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coffee_PryStore.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryTable",
                columns: table => new
                {
                    CategID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategDescript = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTable", x => x.CategID);
                });

            migrationBuilder.CreateTable(
                name: "Table",
                columns: table => new
                {
                    CofId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CofName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CofCateg = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CofPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CofAmount = table.Column<int>(type: "int", nullable: false),
                    CofDuration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => x.CofId);
                    table.ForeignKey(
                        name: "FK_Table_CategoryTable_CofCateg",
                        column: x => x.CofCateg,
                        principalTable: "CategoryTable",
                        principalColumn: "CategID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Table_CofCateg",
                table: "Table",
                column: "CofCateg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.DropTable(
                name: "CategoryTable");
        }
    }
}
