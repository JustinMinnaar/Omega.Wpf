using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class ImportFilesInSubFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportExcludeExtensions",
                table: "SysUserSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ImportFilesInSubFolders",
                table: "SysUserSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportExcludeExtensions",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "ImportFilesInSubFolders",
                table: "SysUserSettings");
        }
    }
}
