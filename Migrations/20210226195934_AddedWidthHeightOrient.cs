using Microsoft.EntityFrameworkCore.Migrations;

namespace ricoai.Migrations
{
    public partial class AddedWidthHeightOrient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "UserImage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Orientation",
                table: "UserImage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "UserImage",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "UserImage");
        }
    }
}
