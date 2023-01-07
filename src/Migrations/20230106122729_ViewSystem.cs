using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HitReFreSH.WebLedger.Migrations
{
    /// <inheritdoc />
    public partial class ViewSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViewTemplates",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Categories = table.Column<string>(type: "varchar(4096)", maxLength: 4096, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsIncome = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewTemplates", x => x.Name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ViewAutomation",
                columns: table => new
                {
                    Type = table.Column<int>(type: "int", nullable: false),
                    TemplateName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewAutomation", x => new { x.TemplateName, x.Type });
                    table.ForeignKey(
                        name: "FK_ViewAutomation_ViewTemplates_TemplateName",
                        column: x => x.TemplateName,
                        principalTable: "ViewTemplates",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Views",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TemplateName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Views", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Views_ViewTemplates_TemplateName",
                        column: x => x.TemplateName,
                        principalTable: "ViewTemplates",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Views_TemplateName",
                table: "Views",
                column: "TemplateName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewAutomation");

            migrationBuilder.DropTable(
                name: "Views");

            migrationBuilder.DropTable(
                name: "ViewTemplates");
        }
    }
}
