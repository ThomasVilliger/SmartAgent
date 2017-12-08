using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartDataHub.Migrations
{
    public partial class @default : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InputMonitoringConfiguration",
                columns: table => new
                {
                    InputMonitoringConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InputPin = table.Column<int>(type: "int", nullable: false),
                    MonitoringName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputPin = table.Column<int>(type: "int", nullable: false),
                    SmartAgentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputMonitoringConfiguration", x => x.InputMonitoringConfigurationId);
                    table.ForeignKey(
                        name: "FK_InputMonitoringConfiguration_SmartAgent_SmartAgentId",
                        column: x => x.SmartAgentId,
                        principalTable: "SmartAgent",
                        principalColumn: "SmartAgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputMonitoringConfiguration_SmartAgentId",
                table: "InputMonitoringConfiguration",
                column: "SmartAgentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputMonitoringConfiguration");
        }
    }
}
