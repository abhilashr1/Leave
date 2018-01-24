using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Leave.Migrations
{
    public partial class Reasons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "LeaveRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Approver",
                table: "LeaveRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "LeaveRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "Approver",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "LeaveRequest");
        }
    }
}
