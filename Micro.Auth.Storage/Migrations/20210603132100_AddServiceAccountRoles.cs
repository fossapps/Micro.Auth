using Microsoft.EntityFrameworkCore.Migrations;

namespace Micro.Auth.Storage.Migrations
{
    public partial class AddServiceAccountRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9a6eb015-82d1-480c-b962-5aab596ef4f6", "642b86b6-bae3-4e08-ae80-1a1dcd007a23", "service_account", "SERVICE_ACCOUNT" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { 1, "token_expiry", "1440", "9a6eb015-82d1-480c-b962-5aab596ef4f6" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a6eb015-82d1-480c-b962-5aab596ef4f6");
        }
    }
}
