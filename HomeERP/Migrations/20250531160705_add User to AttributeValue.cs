using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeERP.Migrations
{
    /// <inheritdoc />
    public partial class addUsertoAttributeValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "StringAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "LinkAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "IntegerAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "FloatAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "FileAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "DateAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StringAttributeValues_UserId",
                table: "StringAttributeValues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkAttributeValues_UserId",
                table: "LinkAttributeValues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegerAttributeValues_UserId",
                table: "IntegerAttributeValues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FloatAttributeValues_UserId",
                table: "FloatAttributeValues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttributeValues_UserId",
                table: "FileAttributeValues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DateAttributeValues_UserId",
                table: "DateAttributeValues",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DateAttributeValues_Users_UserId",
                table: "DateAttributeValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttributeValues_Users_UserId",
                table: "FileAttributeValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FloatAttributeValues_Users_UserId",
                table: "FloatAttributeValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IntegerAttributeValues_Users_UserId",
                table: "IntegerAttributeValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LinkAttributeValues_Users_UserId",
                table: "LinkAttributeValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StringAttributeValues_Users_UserId",
                table: "StringAttributeValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateAttributeValues_Users_UserId",
                table: "DateAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_FileAttributeValues_Users_UserId",
                table: "FileAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_FloatAttributeValues_Users_UserId",
                table: "FloatAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_IntegerAttributeValues_Users_UserId",
                table: "IntegerAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkAttributeValues_Users_UserId",
                table: "LinkAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_StringAttributeValues_Users_UserId",
                table: "StringAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_StringAttributeValues_UserId",
                table: "StringAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_LinkAttributeValues_UserId",
                table: "LinkAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_IntegerAttributeValues_UserId",
                table: "IntegerAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_FloatAttributeValues_UserId",
                table: "FloatAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_FileAttributeValues_UserId",
                table: "FileAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_DateAttributeValues_UserId",
                table: "DateAttributeValues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StringAttributeValues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LinkAttributeValues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IntegerAttributeValues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FloatAttributeValues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FileAttributeValues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DateAttributeValues");
        }
    }
}
