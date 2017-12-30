using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartDataHub.Migrations
{
    public partial class m9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyCycleCoutner",
                table: "MachineStateHistory");

            migrationBuilder.AddColumn<int>(
                name: "DailyCycleCounte",
                table: "MachineStateHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyCycleCounte",
                table: "MachineStateHistory");

            migrationBuilder.AddColumn<int>(
                name: "DailyCycleCoutner",
                table: "MachineStateHistory",
                nullable: false,
                defaultValue: 0);
        }
    }
}
