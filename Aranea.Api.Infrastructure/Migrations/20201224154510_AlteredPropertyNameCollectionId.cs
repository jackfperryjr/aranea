using Microsoft.EntityFrameworkCore.Migrations;

namespace Aranea.Api.Infrastructure.Migrations
{
    public partial class AlteredPropertyNameCollectionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserPhotos_AspNetUsers_CollectionId",
                table: "AspNetUserPhotos");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserPhotos_CollectionId",
                table: "AspNetUserPhotos");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "AspNetUserPhotos");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AspNetUserPhotos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserPhotos_UserId",
                table: "AspNetUserPhotos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserPhotos_AspNetUsers_UserId",
                table: "AspNetUserPhotos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserPhotos_AspNetUsers_UserId",
                table: "AspNetUserPhotos");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserPhotos_UserId",
                table: "AspNetUserPhotos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUserPhotos");

            migrationBuilder.AddColumn<string>(
                name: "CollectionId",
                table: "AspNetUserPhotos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserPhotos_CollectionId",
                table: "AspNetUserPhotos",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserPhotos_AspNetUsers_CollectionId",
                table: "AspNetUserPhotos",
                column: "CollectionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
