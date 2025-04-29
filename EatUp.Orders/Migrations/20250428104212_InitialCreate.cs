using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatUp.Orders.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FoodPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodPackageTitle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FoodPackageTitle_UserName_PaymentStatus_PaymentId",
                table: "Orders",
                columns: new[] { "FoodPackageTitle", "UserName", "PaymentStatus", "PaymentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
