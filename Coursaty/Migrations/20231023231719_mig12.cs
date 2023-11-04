using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coursaty.Migrations
{
    /// <inheritdoc />
    public partial class mig12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "CarouselItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "CarouselItems",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "CarouselItems");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "CarouselItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
