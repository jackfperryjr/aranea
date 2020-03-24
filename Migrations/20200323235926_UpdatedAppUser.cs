using Microsoft.EntityFrameworkCore.Migrations;

namespace Aranea.Migrations
{
    public partial class UpdatedAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        migrationBuilder.RenameColumn(name: "Picture", table: "AspNetUsers", newName: "Photo", schema: "dbo");

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        migrationBuilder.RenameColumn(name: "Photo", table: "AspNetUsers", newName: "Picture", schema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "Age",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
