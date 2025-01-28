using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalonApp.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleMapsIframeToSalon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsIframe",
                table: "Salons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleMapsIframe",
                table: "Salons");
        }
    }
}
