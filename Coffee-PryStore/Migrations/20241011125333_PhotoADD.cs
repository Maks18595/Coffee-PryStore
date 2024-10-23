using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coffee_PryStore.Migrations
{
    /// <inheritdoc />
    public partial class PhotoADD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Table");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Table",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Table");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Table",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
