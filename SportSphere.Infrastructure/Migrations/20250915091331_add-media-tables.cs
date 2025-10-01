using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addmediatables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AthleteAchievements_SocialProfiles_AthleteId",
                table: "AthleteAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_Posts_PostId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_SocialProfiles_UserId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_SocialProfiles_UserId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_SocialProfiles_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialProfiles_AspNetUsers_UserId",
                table: "SocialProfiles");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "88629e30-d94a-4754-9e41-9e3ebf3eec97" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "88629e30-d94a-4754-9e41-9e3ebf3eec97");

            migrationBuilder.CreateTable(
                name: "MediaFolder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFolder_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FolderId = table.Column<long>(type: "bigint", nullable: false),
                    PostId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_MediaFolder_FolderId",
                        column: x => x.FolderId,
                        principalTable: "MediaFolder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Media_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "585ddb2f-f0ad-4a29-97d5-a63bec8ca910", 0, "68499cd7-6c74-4fbe-9b9c-ac7b3a28228f", new DateTime(2025, 9, 15, 9, 13, 30, 519, DateTimeKind.Utc).AddTicks(2401), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAEIdSpuZ1pz5Vn/vhpxvzY0Jki+qNtBF3YCGy5KT7V7aPtte6pGH3nT52tVxU6t78aw==", null, false, null, 0, "2c2ae238-e679-416e-88e3-06c136389fc1", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "585ddb2f-f0ad-4a29-97d5-a63bec8ca910" });

            migrationBuilder.CreateIndex(
                name: "IX_Media_FolderId",
                table: "Media",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_PostId",
                table: "Media",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFolder_UserId",
                table: "MediaFolder",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AthleteAchievements_SocialProfiles_AthleteId",
                table: "AthleteAchievements",
                column: "AthleteId",
                principalTable: "SocialProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_Posts_PostId",
                table: "PostComments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_SocialProfiles_UserId",
                table: "PostComments",
                column: "UserId",
                principalTable: "SocialProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_SocialProfiles_UserId",
                table: "PostLikes",
                column: "UserId",
                principalTable: "SocialProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_SocialProfiles_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "SocialProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialProfiles_AspNetUsers_UserId",
                table: "SocialProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AthleteAchievements_SocialProfiles_AthleteId",
                table: "AthleteAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_Posts_PostId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_SocialProfiles_UserId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_SocialProfiles_UserId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_SocialProfiles_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialProfiles_AspNetUsers_UserId",
                table: "SocialProfiles");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "MediaFolder");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "585ddb2f-f0ad-4a29-97d5-a63bec8ca910" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "585ddb2f-f0ad-4a29-97d5-a63bec8ca910");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "DeletedAt", "Email", "EmailConfirmed", "FullName", "Gender", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImage", "Role", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { "88629e30-d94a-4754-9e41-9e3ebf3eec97", 0, "427de3ef-37ff-416e-8805-4a1e08696e06", new DateTime(2025, 9, 15, 8, 55, 14, 436, DateTimeKind.Utc).AddTicks(3074), new DateTime(1998, 9, 19, 3, 0, 0, 0, DateTimeKind.Local), null, "hobv@gmail.com", true, "hobv", 0, true, false, null, "HOBV@GMAIL.COM", "HOBV@GMAIL.COM", "AQAAAAIAAYagAAAAEEIM87JucPrY0u9uVhwoMfwEYDXKcU4/n5hUfoqnKnPxHiotLnv3Sa/fxvRu19ALsg==", null, false, null, 0, "b2631b6b-9429-4e9b-9928-c3ca48cad857", false, null, "hobv@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "88629e30-d94a-4754-9e41-9e3ebf3eec97" });

            migrationBuilder.AddForeignKey(
                name: "FK_AthleteAchievements_SocialProfiles_AthleteId",
                table: "AthleteAchievements",
                column: "AthleteId",
                principalTable: "SocialProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_Posts_PostId",
                table: "PostComments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_SocialProfiles_UserId",
                table: "PostComments",
                column: "UserId",
                principalTable: "SocialProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_SocialProfiles_UserId",
                table: "PostLikes",
                column: "UserId",
                principalTable: "SocialProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_SocialProfiles_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "SocialProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SocialProfiles_AspNetUsers_UserId",
                table: "SocialProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
