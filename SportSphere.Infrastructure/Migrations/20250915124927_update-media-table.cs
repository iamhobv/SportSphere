using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatemediatable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "585ddb2f-f0ad-4a29-97d5-a63bec8ca910" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "585ddb2f-f0ad-4a29-97d5-a63bec8ca910");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Media",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5", 0, "a4abfb62-02cb-4b1f-9623-ea45d06638ab", new DateTime(2025, 9, 15, 12, 49, 26, 309, DateTimeKind.Utc).AddTicks(79), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAEFzTD7TouFgEqjxsgG/xPXcBQ9uf9O6knx9Teu4mRjE3kOBRKASSVNkhnzoyyGcxsA==", null, false, null, 0, "8afdc3c6-9a3e-4d47-80d7-abb5d518761f", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "33da8d6c-e22b-4cb3-9ac2-60eb0f5d0ea5");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Media");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "585ddb2f-f0ad-4a29-97d5-a63bec8ca910", 0, "68499cd7-6c74-4fbe-9b9c-ac7b3a28228f", new DateTime(2025, 9, 15, 9, 13, 30, 519, DateTimeKind.Utc).AddTicks(2401), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAEIdSpuZ1pz5Vn/vhpxvzY0Jki+qNtBF3YCGy5KT7V7aPtte6pGH3nT52tVxU6t78aw==", null, false, null, 0, "2c2ae238-e679-416e-88e3-06c136389fc1", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "585ddb2f-f0ad-4a29-97d5-a63bec8ca910" });
        }
    }
}
