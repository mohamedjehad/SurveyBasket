using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019f3f9a-67f3-7af8-8d8d-1c5486519f55", "019f3f9a-67f5-7a67-8b09-fc26610f6dcc", false, false, "Admin", "ADMIN" },
                    { "019f3f9a-67f5-7a67-8b09-fc273428ba2f", "019f3f9a-67f5-7a67-8b09-fc28d856f236", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "019f3f7e-73dc-7820-8ea6-280018809133", 0, "019f3f7e-73dc-7820-8ea6-280130d29750", "admin@survey-basket.com", true, "SurveyBasket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAEAACcQAAAAEO4JCMDgMsOsSThzdm4NvV/9um9LTXgwBXqy3VtZXEMpMOJZLjaAOgsA2ir3QJ2OBg==", null, false, "84B03BB714C74CC1AB3A8A0F4EBC983D", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 2, "permissions", "polls:add", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 3, "permissions", "polls:update", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 4, "permissions", "polls:delete", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 5, "permissions", "questions:read", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 6, "permissions", "questions:add", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 7, "permissions", "questions:update", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 8, "permissions", "users:read", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 9, "permissions", "users:add", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 10, "permissions", "users:update", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 11, "permissions", "roles:read", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 12, "permissions", "roles:add", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 13, "permissions", "roles:update", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" },
                    { 14, "permissions", "results:read", "019f3f9a-67f3-7af8-8d8d-1c5486519f55" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019f3f9a-67f3-7af8-8d8d-1c5486519f55", "019f3f7e-73dc-7820-8ea6-280018809133" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019f3f9a-67f5-7a67-8b09-fc273428ba2f");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019f3f9a-67f3-7af8-8d8d-1c5486519f55", "019f3f7e-73dc-7820-8ea6-280018809133" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019f3f9a-67f3-7af8-8d8d-1c5486519f55");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019f3f7e-73dc-7820-8ea6-280018809133");
        }
    }
}
