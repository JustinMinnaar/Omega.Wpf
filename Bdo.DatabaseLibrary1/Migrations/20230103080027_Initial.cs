using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BdoBanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BdoBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BdoBankTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementId = table.Column<int>(type: "int", nullable: false),
                    PageNumber = table.Column<int>(type: "int", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debit = table.Column<decimal>(type: "Decimal(18,2)", nullable: true),
                    Credit = table.Column<decimal>(type: "Decimal(18,2)", nullable: true),
                    Balance = table.Column<decimal>(type: "Decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BdoBankTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BdoEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConsentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FolderName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NumberSource = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NameSource = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DateSource = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BdoEmployees", x => x.Id);
                });

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
                name: "RootMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccount_BdoBanks_BankId",
                        column: x => x.BankId,
                        principalTable: "BdoBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankAccount_BdoEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "BdoEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BdoConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConsentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FolderName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BdoConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BdoConsents_BdoEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "BdoEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "BdoBankCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CardNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    BankAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BdoBankCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BdoBankCards_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BdoBankStatements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceFileName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SourcePageCount = table.Column<int>(type: "int", nullable: false),
                    StatementNumber = table.Column<int>(type: "int", nullable: true),
                    StatementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatementDateText = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    StatementPeriod = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    StatementFrequency = table.Column<int>(type: "int", nullable: false),
                    BalanceOpening = table.Column<decimal>(type: "Decimal(18,2)", nullable: true),
                    BalanceClosing = table.Column<decimal>(type: "Decimal(18,2)", nullable: true),
                    FundsUsed = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    FundsReceived = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    FeesCharged = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address4 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address5 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address6 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    VatNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BdoBankStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BdoBankStatements_BankAccount_AccountId",
                        column: x => x.AccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BdoBankPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankStatementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BdoBankPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BdoBankPages_BdoBankStatements_BankStatementId",
                        column: x => x.BankStatementId,
                        principalTable: "BdoBankStatements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerFolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Length = table.Column<long>(type: "bigint", nullable: false),
                    LastWriteTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PageCount = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocDocument_DocDocumentType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DocDocumentType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceIndex = table.Column<int>(type: "int", nullable: true),
                    SourceLeft = table.Column<double>(type: "float", nullable: true),
                    SourceTop = table.Column<double>(type: "float", nullable: true),
                    SourceWidth = table.Column<double>(type: "float", nullable: true),
                    SourceHeight = table.Column<double>(type: "float", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "DocFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerFolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OcrEngine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OcrDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SymbolCount = table.Column<int>(type: "int", nullable: true),
                    SymbolData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageCount = table.Column<int>(type: "int", nullable: true),
                    DocDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocPages_DocDocument_DocDocumentId",
                        column: x => x.DocDocumentId,
                        principalTable: "DocDocument",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocPages_DocFiles_OwnerFileId",
                        column: x => x.OwnerFileId,
                        principalTable: "DocFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Checksum = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocImages_DocPages_OwnerPageId",
                        column: x => x.OwnerPageId,
                        principalTable: "DocPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerRootId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RootFolderPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsLoadFoldersChecked = table.Column<bool>(type: "bit", nullable: false),
                    FoldersCount = table.Column<int>(type: "int", nullable: true),
                    DocumentsCount = table.Column<int>(type: "int", nullable: true),
                    PagesCount = table.Column<int>(type: "int", nullable: true),
                    ImagesCount = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Root",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectedProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Root", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Root_DocProjects_SelectedProjectId",
                        column: x => x.SelectedProjectId,
                        principalTable: "DocProjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BankId",
                table: "BankAccount",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_EmployeeId",
                table: "BankAccount",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BdoBankCards_BankAccountId",
                table: "BdoBankCards",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BdoBankPages_BankStatementId",
                table: "BdoBankPages",
                column: "BankStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_BdoBankStatements_AccountId",
                table: "BdoBankStatements",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BdoConsents_EmployeeId",
                table: "BdoConsents",
                column: "EmployeeId");

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
                name: "IX_DocFiles_OwnerFolderId",
                table: "DocFiles",
                column: "OwnerFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocFolders_OwnerProjectId",
                table: "DocFolders",
                column: "OwnerProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DocImages_OwnerPageId",
                table: "DocImages",
                column: "OwnerPageId");

            migrationBuilder.CreateIndex(
                name: "IX_DocPages_DocDocumentId",
                table: "DocPages",
                column: "DocDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocPages_OwnerFileId",
                table: "DocPages",
                column: "OwnerFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DocProjects_OwnerRootId",
                table: "DocProjects",
                column: "OwnerRootId");

            migrationBuilder.CreateIndex(
                name: "IX_DocValue_OwnerDocumentId",
                table: "DocValue",
                column: "OwnerDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Root_SelectedProjectId",
                table: "Root",
                column: "SelectedProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocDocument_DocFolders_OwnerFolderId",
                table: "DocDocument",
                column: "OwnerFolderId",
                principalTable: "DocFolders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocFiles_DocFolders_OwnerFolderId",
                table: "DocFiles",
                column: "OwnerFolderId",
                principalTable: "DocFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocFolders_DocProjects_OwnerProjectId",
                table: "DocFolders",
                column: "OwnerProjectId",
                principalTable: "DocProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocProjects_Root_OwnerRootId",
                table: "DocProjects",
                column: "OwnerRootId",
                principalTable: "Root",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Root_DocProjects_SelectedProjectId",
                table: "Root");

            migrationBuilder.DropTable(
                name: "BdoBankCards");

            migrationBuilder.DropTable(
                name: "BdoBankPages");

            migrationBuilder.DropTable(
                name: "BdoBankTransactions");

            migrationBuilder.DropTable(
                name: "BdoConsents");

            migrationBuilder.DropTable(
                name: "DocField");

            migrationBuilder.DropTable(
                name: "DocImages");

            migrationBuilder.DropTable(
                name: "DocValue");

            migrationBuilder.DropTable(
                name: "RootMessages");

            migrationBuilder.DropTable(
                name: "BdoBankStatements");

            migrationBuilder.DropTable(
                name: "DocPages");

            migrationBuilder.DropTable(
                name: "BankAccount");

            migrationBuilder.DropTable(
                name: "DocDocument");

            migrationBuilder.DropTable(
                name: "DocFiles");

            migrationBuilder.DropTable(
                name: "BdoBanks");

            migrationBuilder.DropTable(
                name: "BdoEmployees");

            migrationBuilder.DropTable(
                name: "DocDocumentType");

            migrationBuilder.DropTable(
                name: "DocFolders");

            migrationBuilder.DropTable(
                name: "DocProjects");

            migrationBuilder.DropTable(
                name: "Root");
        }
    }
}
