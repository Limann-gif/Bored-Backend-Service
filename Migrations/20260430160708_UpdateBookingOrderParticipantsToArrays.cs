using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoredWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingOrderParticipantsToArrays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ParticipantsName", table: "ActivityBookingOrders");
            migrationBuilder.DropColumn(name: "ParticipantsEmail", table: "ActivityBookingOrders");

            migrationBuilder.AddColumn<List<string>>(
                name: "ParticipantsName",
                table: "ActivityBookingOrders",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "ParticipantsEmail",
                table: "ActivityBookingOrders",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ParticipantsName", table: "ActivityBookingOrders");
            migrationBuilder.DropColumn(name: "ParticipantsEmail", table: "ActivityBookingOrders");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantsName",
                table: "ActivityBookingOrders",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantsEmail",
                table: "ActivityBookingOrders",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
