using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class ImportProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Path",
                table: "DocProjects",
                newName: "WorkingFolderPath");

            migrationBuilder.AddColumn<string>(
                name: "ImportExcludeWildcards",
                table: "DocProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ImportFilesInSubFolders",
                table: "DocProjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImportFolderPath",
                table: "DocProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImportProgressPercentage",
                table: "DocProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImportStatus",
                table: "DocProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportExcludeWildcards",
                table: "DocProjects");

            migrationBuilder.DropColumn(
                name: "ImportFilesInSubFolders",
                table: "DocProjects");

            migrationBuilder.DropColumn(
                name: "ImportFolderPath",
                table: "DocProjects");

            migrationBuilder.DropColumn(
                name: "ImportProgressPercentage",
                table: "DocProjects");

            migrationBuilder.DropColumn(
                name: "ImportStatus",
                table: "DocProjects");

            migrationBuilder.RenameColumn(
                name: "WorkingFolderPath",
                table: "DocProjects",
                newName: "Path");
        }
    }
}
