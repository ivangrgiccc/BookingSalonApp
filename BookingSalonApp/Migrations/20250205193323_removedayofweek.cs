using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalonApp.Migrations
{
    /// <inheritdoc />
    public partial class removedayofweek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "WorkingHours");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WorkingHours",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WorkingHours",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "WorkingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
