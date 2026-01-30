using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteJournals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "JournalEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JournalEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "JournalEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_UserId",
                table: "JournalEntries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_AspNetUsers_UserId",
                table: "JournalEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_AspNetUsers_UserId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_UserId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "JournalEntries");
        }
    }
}
