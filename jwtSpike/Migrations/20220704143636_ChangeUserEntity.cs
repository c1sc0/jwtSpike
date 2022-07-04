using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jwtSpike.Migrations
{
    public partial class ChangeUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Password", "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { new Guid("40e88163-4289-4183-a114-f45a22fa5532"), "test", "C96YIf7z2fywy5QavCvrlQ==|10240|vvvxh7xY0eCdgoLje0Ugnsf+LmFsoOTsGqGSsq6aun8=|SHA256", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("40e88163-4289-4183-a114-f45a22fa5532"));

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");
        }
    }
}
