using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomatologyClinic.Migrations
{
    /// <inheritdoc />
    public partial class ADDTREAT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialPackageId",
                table: "Treatments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecialPackageId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpecialPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialPackages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_SpecialPackageId",
                table: "Treatments",
                column: "SpecialPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SpecialPackageId",
                table: "Bookings",
                column: "SpecialPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SpecialPackages_SpecialPackageId",
                table: "Bookings",
                column: "SpecialPackageId",
                principalTable: "SpecialPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Treatments_SpecialPackages_SpecialPackageId",
                table: "Treatments",
                column: "SpecialPackageId",
                principalTable: "SpecialPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SpecialPackages_SpecialPackageId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Treatments_SpecialPackages_SpecialPackageId",
                table: "Treatments");

            migrationBuilder.DropTable(
                name: "SpecialPackages");

            migrationBuilder.DropIndex(
                name: "IX_Treatments_SpecialPackageId",
                table: "Treatments");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SpecialPackageId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SpecialPackageId",
                table: "Treatments");

            migrationBuilder.DropColumn(
                name: "SpecialPackageId",
                table: "Bookings");
        }
    }
}
