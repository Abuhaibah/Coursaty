using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coursaty.Migrations
{
    /// <inheritdoc />
    public partial class mig15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuItemMenuId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItems_MenuItemMenuId",
                        column: x => x.MenuItemMenuId,
                        principalTable: "MenuItems",
                        principalColumn: "MenuId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuItemMenuId",
                table: "MenuItems",
                column: "MenuItemMenuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems");
        }
    }
}
