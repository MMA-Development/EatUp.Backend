using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatUp.Vendors.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cvr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<double>(type: "real", nullable: false),
                    Latitude = table.Column<double>(type: "real", nullable: false),
                    StripeAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Name_Longitude_Latitude",
                table: "Vendors",
                columns: new[] { "Name", "Longitude", "Latitude" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_StripeAccountId",
                table: "Vendors",
                column: "StripeAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Username",
                table: "Vendors",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vendors");
        }
    }
}
