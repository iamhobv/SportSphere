using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCounties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "8649835f-ee9a-4801-93dd-2612d929b457" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8649835f-ee9a-4801-93dd-2612d929b457");

            migrationBuilder.AlterColumn<string>(
                name: "Sport",
                table: "SocialProfiles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Media",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "c071192c-1fff-4cd2-bc5b-d096d0b31f1c", 0, "08d4f934-4fa3-44f3-85ec-c90aae4e6ca4", new DateTime(2025, 9, 30, 20, 1, 59, 407, DateTimeKind.Utc).AddTicks(316), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAEDSpY/r+p3ADKy/wXuoYQok6VIZmFhb/sFSEB+frOHLNf1Oemp31BUdJa8RHU09ebQ==", null, false, null, 0, "9eac237c-ae32-466f-a7d9-e9cb018af890", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "c071192c-1fff-4cd2-bc5b-d096d0b31f1c" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialProfiles_Sport",
                table: "SocialProfiles",
                column: "Sport");

            migrationBuilder.CreateIndex(
                name: "IX_Media_FileName",
                table: "Media",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_SocialProfiles_Sport",
                table: "SocialProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Media_FileName",
                table: "Media");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "c071192c-1fff-4cd2-bc5b-d096d0b31f1c" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c071192c-1fff-4cd2-bc5b-d096d0b31f1c");

            migrationBuilder.AlterColumn<string>(
                name: "Sport",
                table: "SocialProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Media",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "8649835f-ee9a-4801-93dd-2612d929b457", 0, "f948c065-b910-4795-b021-31290beb7cfe", new DateTime(2025, 9, 16, 0, 13, 41, 584, DateTimeKind.Utc).AddTicks(4376), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAEE3VUOBzXSznguaFoSxFJlFRno2+vaV4lcIeit4qATFxbFRKzfZdkdk7XQKPO56sSw==", null, false, null, 0, "d0774529-9860-4df8-a036-1aaa96cd5af6", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "8649835f-ee9a-4801-93dd-2612d929b457" });
        }
    }
}
