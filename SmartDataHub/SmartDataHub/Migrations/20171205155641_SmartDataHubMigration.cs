using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartDataHub.Migrations
{
    public partial class SmartDataHubMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmartAgent",
                columns: table => new
                {
                    SmartAgentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartAgent", x => x.SmartAgentId);
                });

            migrationBuilder.CreateTable(
                name: "CycleMachineConfiguration",
                columns: table => new
                {
                    CycleMachineConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CycleInputPin = table.Column<int>(type: "int", nullable: false),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineStateTimeOut = table.Column<int>(type: "int", nullable: false),
                    PublishingIntervall = table.Column<int>(type: "int", nullable: false),
                    SmartAgentId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_CycleMachineConfiguration_SmartAgentId",
                table: "CycleMachineConfiguration",
                column: "SmartAgentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CycleMachineConfiguration");

            migrationBuilder.DropTable(
                name: "SmartAgent");
        }
    }
}
