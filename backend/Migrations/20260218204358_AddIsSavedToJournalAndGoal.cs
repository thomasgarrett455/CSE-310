using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSavedToJournalAndGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_Categories_CategoryId",
                table: "JournalEntries");

            migrationBuilder.DropTable(
                name: "JournalEntryTag");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_CategoryId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "JournalEntries");

            migrationBuilder.AddColumn<bool>(
                name: "IsSaved",
                table: "JournalEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSaved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalCategories",
                columns: table => new
                {
                    JournalEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalCategories", x => new { x.JournalEntryId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_JournalCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalCategories_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalTags",
                columns: table => new
                {
                    JournalEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalTags", x => new { x.JournalEntryId, x.TagId });
                    table.ForeignKey(
                        name: "FK_JournalTags_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalCategories_CategoryId",
                table: "JournalCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalTags_TagId",
                table: "JournalTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "JournalCategories");

            migrationBuilder.DropTable(
                name: "JournalTags");

            migrationBuilder.DropColumn(
                name: "IsSaved",
                table: "JournalEntries");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "JournalEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JournalEntryTag",
                columns: table => new
                {
                    JournalsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryTag", x => new { x.JournalsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_JournalEntryTag_JournalEntries_JournalsId",
                        column: x => x.JournalsId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_CategoryId",
                table: "JournalEntries",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryTag_TagsId",
                table: "JournalEntryTag",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_Categories_CategoryId",
                table: "JournalEntries",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
