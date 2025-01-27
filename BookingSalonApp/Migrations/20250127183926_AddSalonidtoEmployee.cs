using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalonApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSalonidtoEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Salons_SalonId",
                table: "Employees");

            migrationBuilder.AlterColumn<int>(
                name: "SalonId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
    name: "FK_Employees_Salons_SalonId",
    table: "Employees",
    column: "SalonId",
    principalTable: "Salons",
    principalColumn: "Id",
    onDelete: ReferentialAction.NoAction,
    onUpdate: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Salons_SalonId",
                table: "Employees");

            migrationBuilder.AlterColumn<int>(
                name: "SalonId",
                table: "Employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Salons_SalonId",
                table: "Employees",
                column: "SalonId",
                principalTable: "Salons",
                principalColumn: "Id");
        }
    }
}
