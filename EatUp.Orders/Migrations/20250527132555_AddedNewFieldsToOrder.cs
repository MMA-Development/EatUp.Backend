using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatUp.Orders.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "UserProjections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EphemeralKey",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentSecret",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "UserProjections");

            migrationBuilder.DropColumn(
                name: "EphemeralKey",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentSecret",
                table: "Orders");
        }
    }
}
