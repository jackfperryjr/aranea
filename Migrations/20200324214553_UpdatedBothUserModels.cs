using Microsoft.EntityFrameworkCore.Migrations;

namespace Aranea.Migrations
{
    public partial class UpdatedBothUserModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "AspNetUsers");
        }
    }
}
