using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkShopHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAbVariantToLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbVariant",
                table: "Links",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Variant",
                table: "ClickEvents",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomDomains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Domain = table.Column<string>(type: "text", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomDomains", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomDomains");

            migrationBuilder.DropColumn(
                name: "AbVariant",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "Variant",
                table: "ClickEvents");
        }
    }
}
