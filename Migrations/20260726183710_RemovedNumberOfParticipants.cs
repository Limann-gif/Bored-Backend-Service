using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoredWeb.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNumberOfParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfParticipants",
                table: "Activities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfParticipants",
                table: "Activities",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
