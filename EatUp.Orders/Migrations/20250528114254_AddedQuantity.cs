using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatUp.Orders.Migrations
{
    /// <inheritdoc />
    public partial class AddedQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "MealProjections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "MealProjections");
        }
    }
}
