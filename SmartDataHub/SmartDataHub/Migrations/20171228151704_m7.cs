using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartDataHub.Migrations
{
    public partial class m7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineStateHistory_CycleMachineConfiguration_CycleMachineConfigurationId",
                table: "MachineStateHistory");

            migrationBuilder.DropTable(
                name: "CycleMachineConfiguration");

            migrationBuilder.DropTable(
                name: "InputMonitoringConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_MachineStateHistory_CycleMachineConfigurationId",
                table: "MachineStateHistory");

            migrationBuilder.DropColumn(
                name: "CycleMachineConfigurationId",
                table: "MachineStateHistory");

            migrationBuilder.AddColumn<int>(
                name: "MachineId",
                table: "MachineStateHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InputMonitoring",
                columns: table => new
                {
                    InputMonitoringId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InputPin = table.Column<int>(type: "int", nullable: false),
                    MonitoringName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputPin = table.Column<int>(type: "int", nullable: false),
                    SmartAgentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputMonitoring", x => x.InputMonitoringId);
                    table.ForeignKey(
                        name: "FK_InputMonitoring_SmartAgent_SmartAgentId",
                        column: x => x.SmartAgentId,
                        principalTable: "SmartAgent",
                        principalColumn: "SmartAgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Machine",
                columns: table => new
                {
                    MachineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CycleInputPin = table.Column<int>(type: "int", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineStateTimeOut = table.Column<int>(type: "int", nullable: false),
                    PublishingIntervall = table.Column<int>(type: "int", nullable: false),
                    SmartAgentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machine", x => x.MachineId);
                    table.ForeignKey(
                        name: "FK_Machine_SmartAgent_SmartAgentId",
                        column: x => x.SmartAgentId,
                        principalTable: "SmartAgent",
                        principalColumn: "SmartAgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachineStateHistory_MachineId",
                table: "MachineStateHistory",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_InputMonitoring_SmartAgentId",
                table: "InputMonitoring",
                column: "SmartAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Machine_SmartAgentId",
                table: "Machine",
                column: "SmartAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineStateHistory_Machine_MachineId",
                table: "MachineStateHistory",
                column: "MachineId",
                principalTable: "Machine",
                principalColumn: "MachineId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineStateHistory_Machine_MachineId",
                table: "MachineStateHistory");

            migrationBuilder.DropTable(
                name: "InputMonitoring");

            migrationBuilder.DropTable(
                name: "Machine");

            migrationBuilder.DropIndex(
                name: "IX_MachineStateHistory_MachineId",
                table: "MachineStateHistory");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "MachineStateHistory");

            migrationBuilder.AddColumn<int>(
                name: "CycleMachineConfigurationId",
                table: "MachineStateHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CycleMachineConfiguration",
                columns: table => new
                {
                    CycleMachineConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CycleInputPin = table.Column<int>(nullable: false),
                    MachineName = table.Column<string>(nullable: false),
                    MachineStateTimeOut = table.Column<int>(nullable: false),
                    PublishingIntervall = table.Column<int>(nullable: false),
                    SmartAgentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CycleMachineConfiguration", x => x.CycleMachineConfigurationId);
                    table.ForeignKey(
                        name: "FK_CycleMachineConfiguration_SmartAgent_SmartAgentId",
                        column: x => x.SmartAgentId,
                        principalTable: "SmartAgent",
                        principalColumn: "SmartAgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InputMonitoringConfiguration",
                columns: table => new
                {
                    InputMonitoringConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    InputPin = table.Column<int>(nullable: false),
                    MonitoringName = table.Column<string>(nullable: false),
                    OutputPin = table.Column<int>(nullable: false),
                    SmartAgentId = table.Column<int>(nullable: false)
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
                name: "IX_MachineStateHistory_CycleMachineConfigurationId",
                table: "MachineStateHistory",
                column: "CycleMachineConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleMachineConfiguration_SmartAgentId",
                table: "CycleMachineConfiguration",
                column: "SmartAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_InputMonitoringConfiguration_SmartAgentId",
                table: "InputMonitoringConfiguration",
                column: "SmartAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineStateHistory_CycleMachineConfiguration_CycleMachineConfigurationId",
                table: "MachineStateHistory",
                column: "CycleMachineConfigurationId",
                principalTable: "CycleMachineConfiguration",
                principalColumn: "CycleMachineConfigurationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
