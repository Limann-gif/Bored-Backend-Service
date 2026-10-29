using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoredWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordVerificationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordVerificationCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordVerificationCodeExpiry",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordVerificationCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordVerificationCodeExpiry",
                table: "Users");
        }
    }
}
