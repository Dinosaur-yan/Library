using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Library.API.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    BirthDate = table.Column<DateTimeOffset>(nullable: false),
                    BirthPlace = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Pages = table.Column<int>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "BirthDate", "BirthPlace", "Email", "Name" },
                values: new object[] { new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c"), new DateTimeOffset(new DateTime(1960, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), null, "author1@xxx.com", "Author 1" });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "BirthDate", "BirthPlace", "Email", "Name" },
                values: new object[] { new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9"), new DateTimeOffset(new DateTime(1973, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), null, "author2@xxx.com", "Author 2" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Description", "Pages", "Title" },
                values: new object[,]
                {
                    { new Guid("a87f37d4-70c1-4d8a-bd34-76ca4194982f"), new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c"), "Description of Book 1", 281, "Book 1" },
                    { new Guid("c3357824-6d12-4a41-a544-76659e848263"), new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c"), "Description of Book 2", 370, "Book 2" },
                    { new Guid("e1108d59-2bc3-4f2f-b643-f3d0cb3b7e0b"), new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9"), "Description of Book 3", 229, "Book 3" },
                    { new Guid("f1ea8f47-aed7-4c33-a1c7-8a96abfc89aa"), new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9"), "Description of Book 4", 440, "Book 4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");
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
