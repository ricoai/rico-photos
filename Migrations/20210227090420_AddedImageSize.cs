using Microsoft.EntityFrameworkCore.Migrations;

namespace ricoai.Migrations
{
    public partial class AddedImageSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                table: "UserImage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "FileSizeStr",
                table: "UserImage",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "FileSizeStr",
                table: "UserImage");
        }
    }
}
