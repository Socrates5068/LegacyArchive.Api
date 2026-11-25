using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyArchive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "MediaFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreviewPath",
                table: "MediaFiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "MediaFiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "MediaFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "PreviewPath",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "MediaFiles");
        }
    }
}
