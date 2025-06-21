using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomatologyClinic.Migrations
{
    /// <inheritdoc />
    public partial class makeTreatNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SpecialPackages_SpecialPackageId",
                table: "Bookings");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialPackageId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SpecialPackages_SpecialPackageId",
                table: "Bookings",
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

            migrationBuilder.AlterColumn<int>(
                name: "SpecialPackageId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SpecialPackages_SpecialPackageId",
                table: "Bookings",
                column: "SpecialPackageId",
                principalTable: "SpecialPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
