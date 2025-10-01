using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatefollowtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_SocialProfiles_FollowerId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_SocialProfiles_FollowingId",
                table: "Follows");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5");

            migrationBuilder.AlterColumn<string>(
                name: "FollowingId",
                table: "Follows",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "FollowerId",
                table: "Follows",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "3b8828e9-f617-4fce-83b3-914704200d94", 0, "703f92b4-f083-430e-8576-693f883f8588", new DateTime(2025, 9, 15, 14, 59, 59, 41, DateTimeKind.Utc).AddTicks(5226), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAENoVnlQvmTopWIPJy968UiLw/kq/uY0qCLNpKkH+pR2skkMEj58qyDcdDG5pR/FPiQ==", null, false, null, 0, "dd3daa14-a939-473c-9683-668d94d1e268", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "3b8828e9-f617-4fce-83b3-914704200d94" });

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowingId",
                table: "Follows",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowingId",
                table: "Follows");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "3b8828e9-f617-4fce-83b3-914704200d94" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3b8828e9-f617-4fce-83b3-914704200d94");

            migrationBuilder.AlterColumn<long>(
                name: "FollowingId",
                table: "Follows",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<long>(
                name: "FollowerId",
                table: "Follows",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5", 0, "a4abfb62-02cb-4b1f-9623-ea45d06638ab", new DateTime(2025, 9, 15, 12, 49, 26, 309, DateTimeKind.Utc).AddTicks(79), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAEFzTD7TouFgEqjxsgG/xPXcBQ9uf9O6knx9Teu4mRjE3kOBRKASSVNkhnzoyyGcxsA==", null, false, null, 0, "8afdc3c6-9a3e-4d47-80d7-abb5d518761f", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5" });

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_SocialProfiles_FollowerId",
                table: "Follows",
                column: "FollowerId",
                principalTable: "SocialProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_SocialProfiles_FollowingId",
                table: "Follows",
                column: "FollowingId",
                principalTable: "SocialProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
