using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aranea.Api.Infrastructure.Migrations
{
    public partial class AddedPhotoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUserPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Portrait = table.Column<int>(nullable: false),
                    Wallpaper = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    CollectionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserPhotos_AspNetUsers_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserPhotos_CollectionId",
                table: "AspNetUserPhotos",
                column: "CollectionId");

            migrationBuilder.DropColumn(
                name: "Wallpaper",
                table: "AspNetUsers");
                                
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserPhotos");

            migrationBuilder.AddColumn<string>(
                name: "Wallpaper",
                table: "AspNetUsers",
                nullable: true);
                                
            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
