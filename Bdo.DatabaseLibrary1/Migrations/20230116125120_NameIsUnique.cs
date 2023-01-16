using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class NameIsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "DocProjects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SysUserSettings_Name",
                table: "SysUserSettings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocSolutions_Name",
                table: "DocSolutions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocProjects_Name",
                table: "DocProjects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocFolders_Name",
                table: "DocFolders",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocFiles_Name",
                table: "DocFiles",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SysUserSettings_Name",
                table: "SysUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_DocSolutions_Name",
                table: "DocSolutions");

            migrationBuilder.DropIndex(
                name: "IX_DocProjects_Name",
                table: "DocProjects");

            migrationBuilder.DropIndex(
                name: "IX_DocFolders_Name",
                table: "DocFolders");

            migrationBuilder.DropIndex(
                name: "IX_DocFiles_Name",
                table: "DocFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "DocProjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
