using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class DocFolderAreFilesLoaded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AreFilesLoaded",
                table: "DocFolders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreFilesLoaded",
                table: "DocFolders");
        }
    }
}
