using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeERP.Migrations
{
    /// <inheritdoc />
    public partial class addTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chores_Attributes_AttributeId",
                table: "Chores");

            migrationBuilder.DropIndex(
                name: "IX_Chores_AttributeId",
                table: "Chores");

            migrationBuilder.DropColumn(
                name: "AttributeId",
                table: "Chores");

            migrationBuilder.DropColumn(
                name: "DeltaTimeInDays",
                table: "Chores");

            migrationBuilder.RenameColumn(
                name: "WarningType",
                table: "Chores",
                newName: "Type");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DeltaTime",
                table: "Chores",
                type: "interval",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExecutionMoment = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrackedTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_Chores_ChoreId",
                        column: x => x.ChoreId,
                        principalTable: "Chores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Task_ChoreId",
                table: "Task",
                column: "ChoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ObjectId",
                table: "Task",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_UserId",
                table: "Task",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropColumn(
                name: "DeltaTime",
                table: "Chores");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Chores",
                newName: "WarningType");

            migrationBuilder.AddColumn<Guid>(
                name: "AttributeId",
                table: "Chores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "DeltaTimeInDays",
                table: "Chores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chores_AttributeId",
                table: "Chores",
                column: "AttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chores_Attributes_AttributeId",
                table: "Chores",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
