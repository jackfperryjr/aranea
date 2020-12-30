using Microsoft.EntityFrameworkCore.Migrations;

namespace Aranea.Api.Infrastructure.Migrations
{
    public partial class RefactoredUserModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(table: "AspNetUsers", name: "FirstName", newName: "Name");
            migrationBuilder.RenameColumn(table: "AspNetUsers", name: "Profile", newName: "Bio");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(table: "AspNetUsers", name: "Name", newName: "FirstName");
            migrationBuilder.RenameColumn(table: "AspNetUsers", name: "Bio", newName: "Profile");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
