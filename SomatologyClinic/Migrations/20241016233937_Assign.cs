using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomatologyClinic.Migrations
{
    /// <inheritdoc />
    public partial class Assign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TherapistId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Therapists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Therapists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TherapistId",
                table: "Bookings",
                column: "TherapistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Therapists_TherapistId",
                table: "Bookings",
                column: "TherapistId",
                principalTable: "Therapists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Therapists_TherapistId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Therapists");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TherapistId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TherapistId",
                table: "Bookings");
        }
    }
}
