using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoredWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddedAmountPaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "ActivityBookingOrders",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "ActivityBookingOrders");
        }
    }
}
