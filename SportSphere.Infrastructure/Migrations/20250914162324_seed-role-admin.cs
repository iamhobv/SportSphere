using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedroleadmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "SuperAdmin", "SUPERADMIN" },
                    { "2", null, "Athlete", "ATHLETE" },
                    { "3", null, "Scout", "SCOUT" },
                    { "4", null, "Coach", "COACH" },
                    { "5", null, "Fan", "FAN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "4eaf5f0b-280a-4e70-9779-7726c38ba0c0", 0, "ff78b619-d555-4ae6-9ed0-39b24ed0243e", new DateTime(2025, 9, 14, 16, 23, 24, 152, DateTimeKind.Utc).AddTicks(4848), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAELGxNorOO4ReClhZzgDAPSM0M7g1c72dUxnFmr4e4V79Q+Dz6jH/wZMlAB8Ab1sB0g==", null, false, null, 0, "5ae1fbab-9b22-4bcd-b8f6-d5fe2738a9a6", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "4eaf5f0b-280a-4e70-9779-7726c38ba0c0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "4eaf5f0b-280a-4e70-9779-7726c38ba0c0" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4eaf5f0b-280a-4e70-9779-7726c38ba0c0");
        }
    }
}
