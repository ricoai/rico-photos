using Microsoft.EntityFrameworkCore.Migrations;

namespace ricoai.Migrations
{
    public partial class AddedMeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaData",
                table: "UserImage",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaData",
                table: "UserImage");
        }
    }
}
