using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bdo.DatabaseLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class Profiling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlank",
                table: "DocPages",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsError",
                table: "DocPages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PageIndex",
                table: "DocPages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "DocPages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileName",
                table: "DocPages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ProfileVersion",
                table: "DocPages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsIdentified",
                table: "DocFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "DocFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileName",
                table: "DocFiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlank",
                table: "DocPages");

            migrationBuilder.DropColumn(
                name: "IsError",
                table: "DocPages");

            migrationBuilder.DropColumn(
                name: "PageIndex",
                table: "DocPages");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "DocPages");

            migrationBuilder.DropColumn(
                name: "ProfileName",
                table: "DocPages");

            migrationBuilder.DropColumn(
                name: "ProfileVersion",
                table: "DocPages");

            migrationBuilder.DropColumn(
                name: "IsIdentified",
                table: "DocFiles");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "DocFiles");

            migrationBuilder.DropColumn(
                name: "ProfileName",
                table: "DocFiles");
        }
    }
}
