using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Leave.Migrations
{
    public partial class requestTimeAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Approved",
                table: "LeaveRequest",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestTime",
                table: "LeaveRequest",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestTime",
                table: "LeaveRequest");

            migrationBuilder.AlterColumn<bool>(
                name: "Approved",
                table: "LeaveRequest",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
