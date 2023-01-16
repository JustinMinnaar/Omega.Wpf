using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class ImportFolderPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportFolderPath",
                table: "SysUserSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ImportFoldersInParallel",
                table: "SysUserSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "DocProjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportFolderPath",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "ImportFoldersInParallel",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "DocProjects");
        }
    }
}
