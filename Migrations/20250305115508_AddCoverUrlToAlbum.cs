using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundScape.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverUrlToAlbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Albums",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Albums");
        }
    }
}
