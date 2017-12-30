using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartDataHub.Migrations
{
    public partial class m11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportedHistoryTill",
                table: "SmartAgent");

            migrationBuilder.AddColumn<int>(
                name: "LastSmartAgentHistoryId",
                table: "SmartAgent",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SmartAgentHistoryId",
                table: "MachineStateHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSmartAgentHistoryId",
                table: "SmartAgent");

            migrationBuilder.DropColumn(
                name: "SmartAgentHistoryId",
                table: "MachineStateHistory");

            migrationBuilder.AddColumn<int>(
                name: "ImportedHistoryTill",
                table: "SmartAgent",
                nullable: false,
                defaultValue: 0);
        }
    }
}
