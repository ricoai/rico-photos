using Microsoft.EntityFrameworkCore.Migrations;

namespace ricoai.Migrations
{
    public partial class AddedAiTagsPublic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiFacialTags",
                table: "UserImage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiModerationTags",
                table: "UserImage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiObjectsTags",
                table: "UserImage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiTextInImageTags",
                table: "UserImage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "UserImage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "UserImage",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiFacialTags",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "AiModerationTags",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "AiObjectsTags",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "AiTextInImageTags",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "UserImage");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "UserImage");
        }
    }
}
