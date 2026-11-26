using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyArchive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFileSortIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ArtworkId",
                table: "MediaFiles");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ArtworkId_SortOrder",
                table: "MediaFiles",
                columns: new[] { "ArtworkId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ArtworkId_SortOrder",
                table: "MediaFiles");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ArtworkId",
                table: "MediaFiles",
                column: "ArtworkId");
        }
    }
}
