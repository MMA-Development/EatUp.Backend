using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace EatUp.Vendors.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vendors_Name_Longitude_Latitude",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Vendors");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Vendors",
                type: "geography",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Name",
                table: "Vendors",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vendors_Name",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Vendors");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Vendors",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Vendors",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Name_Longitude_Latitude",
                table: "Vendors",
                columns: new[] { "Name", "Longitude", "Latitude" });
        }
    }
}
