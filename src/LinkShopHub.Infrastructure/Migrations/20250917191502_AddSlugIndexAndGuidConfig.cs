using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkShopHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugIndexAndGuidConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Slug",
                table: "AspNetUsers",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Slug",
                table: "AspNetUsers");
        }
    }
}
