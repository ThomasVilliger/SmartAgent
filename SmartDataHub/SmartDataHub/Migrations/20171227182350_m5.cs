using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartDataHub.Migrations
{
    public partial class m5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MachineStateHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CycleMachineConfigurationId = table.Column<int>(type: "int", nullable: false),
                    CyclesInThisPeriod = table.Column<int>(type: "int", nullable: false),
                    DailyCycleCoutner = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MachineState = table.Column<int>(type: "int", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineStateHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineStateHistory_CycleMachineConfiguration_CycleMachineConfigurationId",
                        column: x => x.CycleMachineConfigurationId,
                        principalTable: "CycleMachineConfiguration",
                        principalColumn: "CycleMachineConfigurationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachineStateHistory_CycleMachineConfigurationId",
                table: "MachineStateHistory",
                column: "CycleMachineConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineStateHistory");
        }
    }
}
