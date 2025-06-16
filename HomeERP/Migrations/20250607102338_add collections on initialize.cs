using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeERP.Migrations
{
    /// <inheritdoc />
    public partial class addcollectionsoninitialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Bundles",
                columns: new[] { "Id", "Discriminator", "Name" },
                values: new object[,]
                {
                    { new Guid("c2703327-15ec-4b67-96f0-be16467a9dbf"), "ShoppingList", "Список покупок" },
                    { new Guid("f6703327-15ec-4b67-96f0-be16467a9dbf"), "Inventory", "Инвентарь" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bundles",
                keyColumn: "Id",
                keyValue: new Guid("c2703327-15ec-4b67-96f0-be16467a9dbf"));

            migrationBuilder.DeleteData(
                table: "Bundles",
                keyColumn: "Id",
                keyValue: new Guid("f6703327-15ec-4b67-96f0-be16467a9dbf"));
        }
    }
}
