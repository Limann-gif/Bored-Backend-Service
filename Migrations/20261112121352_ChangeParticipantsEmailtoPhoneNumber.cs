using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoredWeb.Migrations
{
    /// <inheritdoc />
    public partial class ChangeParticipantsEmailtoPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParticipantsEmail",
                table: "ActivityBookingOrders",
                newName: "ParticipantsPhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParticipantsPhoneNumber",
                table: "ActivityBookingOrders",
                newName: "ParticipantsEmail");
        }
    }
}
