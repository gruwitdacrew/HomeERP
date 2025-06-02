using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeERP.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkedEntityprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkAttributes");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Attributes",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LinkedEntityId",
                table: "Attributes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_LinkedEntityId",
                table: "Attributes",
                column: "LinkedEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attributes_Entities_LinkedEntityId",
                table: "Attributes",
                column: "LinkedEntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attributes_Entities_LinkedEntityId",
                table: "Attributes");

            migrationBuilder.DropIndex(
                name: "IX_Attributes_LinkedEntityId",
                table: "Attributes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Attributes");

            migrationBuilder.DropColumn(
                name: "LinkedEntityId",
                table: "Attributes");

            migrationBuilder.CreateTable(
                name: "LinkAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkedEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkAttributes_Attributes_Id",
                        column: x => x.Id,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkAttributes_Entities_LinkedEntityId",
                        column: x => x.LinkedEntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkAttributes_LinkedEntityId",
                table: "LinkAttributes",
                column: "LinkedEntityId");
        }
    }
}
