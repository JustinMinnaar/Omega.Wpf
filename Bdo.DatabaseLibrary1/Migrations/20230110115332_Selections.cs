using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class Selections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocPages_DocDocument_DocDocumentId",
                table: "DocPages");

            migrationBuilder.DropTable(
                name: "DocField");

            migrationBuilder.DropTable(
                name: "DocValue");

            migrationBuilder.DropTable(
                name: "DocDocument");

            migrationBuilder.DropTable(
                name: "DocDocumentType");

            migrationBuilder.DropIndex(
                name: "IX_DocPages_DocDocumentId",
                table: "DocPages");

            migrationBuilder.DropColumn(
                name: "SelectedDocFileId",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "SelectedDocFolderId",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "SelectedDocPageId",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "SelectedDocProjectId",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "SelectedProProfileId",
                table: "SysUserSettings");

            migrationBuilder.DropColumn(
                name: "DocDocumentId",
                table: "DocPages");

            migrationBuilder.RenameColumn(
                name: "SelectedProTemplateId",
                table: "SysUserSettings",
                newName: "SelectedProBagId");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocProjectId",
                table: "DocSolutions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocFolderId",
                table: "DocProjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocFileId",
                table: "DocFolders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocPageId",
                table: "DocFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysUserSettings_SelectedDocSolutionId",
                table: "SysUserSettings",
                column: "SelectedDocSolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocSolutions_SelectedDocProjectId",
                table: "DocSolutions",
                column: "SelectedDocProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DocProjects_SelectedDocFolderId",
                table: "DocProjects",
                column: "SelectedDocFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocFolders_SelectedDocFileId",
                table: "DocFolders",
                column: "SelectedDocFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DocFiles_SelectedDocPageId",
                table: "DocFiles",
                column: "SelectedDocPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocFiles_DocPages_SelectedDocPageId",
                table: "DocFiles",
                column: "SelectedDocPageId",
                principalTable: "DocPages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocFolders_DocFiles_SelectedDocFileId",
                table: "DocFolders",
                column: "SelectedDocFileId",
                principalTable: "DocFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocProjects_DocFolders_SelectedDocFolderId",
                table: "DocProjects",
                column: "SelectedDocFolderId",
                principalTable: "DocFolders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocSolutions_DocProjects_SelectedDocProjectId",
                table: "DocSolutions",
                column: "SelectedDocProjectId",
                principalTable: "DocProjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SysUserSettings_DocSolutions_SelectedDocSolutionId",
                table: "SysUserSettings",
                column: "SelectedDocSolutionId",
                principalTable: "DocSolutions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocFiles_DocPages_SelectedDocPageId",
                table: "DocFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DocFolders_DocFiles_SelectedDocFileId",
                table: "DocFolders");

            migrationBuilder.DropForeignKey(
                name: "FK_DocProjects_DocFolders_SelectedDocFolderId",
                table: "DocProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_DocSolutions_DocProjects_SelectedDocProjectId",
                table: "DocSolutions");

            migrationBuilder.DropForeignKey(
                name: "FK_SysUserSettings_DocSolutions_SelectedDocSolutionId",
                table: "SysUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_SysUserSettings_SelectedDocSolutionId",
                table: "SysUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_DocSolutions_SelectedDocProjectId",
                table: "DocSolutions");

            migrationBuilder.DropIndex(
                name: "IX_DocProjects_SelectedDocFolderId",
                table: "DocProjects");

            migrationBuilder.DropIndex(
                name: "IX_DocFolders_SelectedDocFileId",
                table: "DocFolders");

            migrationBuilder.DropIndex(
                name: "IX_DocFiles_SelectedDocPageId",
                table: "DocFiles");

            migrationBuilder.DropColumn(
                name: "SelectedDocProjectId",
                table: "DocSolutions");

            migrationBuilder.DropColumn(
                name: "SelectedDocFolderId",
                table: "DocProjects");

            migrationBuilder.DropColumn(
                name: "SelectedDocFileId",
                table: "DocFolders");

            migrationBuilder.DropColumn(
                name: "SelectedDocPageId",
                table: "DocFiles");

            migrationBuilder.RenameColumn(
                name: "SelectedProBagId",
                table: "SysUserSettings",
                newName: "SelectedProTemplateId");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocFileId",
                table: "SysUserSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocFolderId",
                table: "SysUserSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocPageId",
                table: "SysUserSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDocProjectId",
                table: "SysUserSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedProProfileId",
                table: "SysUserSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocDocumentId",
                table: "DocPages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocDocumentType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocDocumentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerFolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastWriteTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Length = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PageCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocDocument_DocDocumentType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DocDocumentType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocDocument_DocFolders_OwnerFolderId",
                        column: x => x.OwnerFolderId,
                        principalTable: "DocFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocField",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocDocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocField_DocDocumentType_DocDocumentTypeId",
                        column: x => x.DocDocumentTypeId,
                        principalTable: "DocDocumentType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceHeight = table.Column<double>(type: "float", nullable: true),
                    SourceIndex = table.Column<int>(type: "int", nullable: true),
                    SourceLeft = table.Column<double>(type: "float", nullable: true),
                    SourceTop = table.Column<double>(type: "float", nullable: true),
                    SourceWidth = table.Column<double>(type: "float", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocValue_DocDocument_OwnerDocumentId",
                        column: x => x.OwnerDocumentId,
                        principalTable: "DocDocument",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocPages_DocDocumentId",
                table: "DocPages",
                column: "DocDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocDocument_OwnerFolderId",
                table: "DocDocument",
                column: "OwnerFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocDocument_TypeId",
                table: "DocDocument",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocField_DocDocumentTypeId",
                table: "DocField",
                column: "DocDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocValue_OwnerDocumentId",
                table: "DocValue",
                column: "OwnerDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocPages_DocDocument_DocDocumentId",
                table: "DocPages",
                column: "DocDocumentId",
                principalTable: "DocDocument",
                principalColumn: "Id");
        }
    }
}
