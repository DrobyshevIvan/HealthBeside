using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthBeside.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixesAddedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAtUtc",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAtUtc",
                table: "AspNetUsers");
        }
    }
}
