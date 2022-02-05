using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OData_webapi_netcore6.Migrations
{
    public partial class initiL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    AuthorGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    HomeState = table.Column<string>(type: "TEXT", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.AuthorGuid);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorGuid = table.Column<Guid>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    PublishYear = table.Column<int>(type: "INTEGER", nullable: false),
                    CopiesSold = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookGuid);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorGuid",
                        column: x => x.AuthorGuid,
                        principalTable: "Authors",
                        principalColumn: "AuthorGuid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorGuid",
                table: "Books",
                column: "AuthorGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
