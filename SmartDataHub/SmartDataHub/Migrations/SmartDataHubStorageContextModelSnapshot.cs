﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SmartDataHub.Models;
using System;

namespace SmartDataHub.Migrations
{
    [DbContext(typeof(SmartDataHubStorageContext))]
    partial class SmartDataHubStorageContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmartDataHub.Models.InputMonitoring", b =>
                {
                    b.Property<int>("InputMonitoringId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("InputPin");

                    b.Property<string>("MonitoringName")
                        .IsRequired();

                    b.Property<int>("OutputPin");

                    b.Property<int>("SmartAgentId");

                    b.HasKey("InputMonitoringId");

                    b.HasIndex("SmartAgentId");

                    b.ToTable("InputMonitoring");
                });

            modelBuilder.Entity("SmartDataHub.Models.Machine", b =>
                {
                    b.Property<int>("MachineId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("CycleInputPin");

                    b.Property<string>("MachineName")
                        .IsRequired();

                    b.Property<int>("MachineStateTimeOut");

                    b.Property<int>("PublishingIntervall");

                    b.Property<int>("SmartAgentId");

                    b.HasKey("MachineId");

                    b.HasIndex("SmartAgentId");

                    b.ToTable("Machine");
                });

            modelBuilder.Entity("SmartDataHub.Models.MachineStateHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CyclesInThisPeriod");

                    b.Property<int>("DailyCycleCounter");

                    b.Property<TimeSpan>("Duration");

                    b.Property<DateTime>("EndDateTime");

                    b.Property<int>("MachineId");

                    b.Property<int>("MachineState");

                    b.Property<int>("SmartAgentHistoryId");

                    b.Property<DateTime>("StartDateTime");

                    b.HasKey("Id");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineStateHistory");
                });

            modelBuilder.Entity("SmartDataHub.Models.SmartAgent", b =>
                {
                    b.Property<int>("SmartAgentId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IpAddress")
                        .IsRequired();

                    b.Property<int>("LastSmartAgentHistoryId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Priority");

                    b.HasKey("SmartAgentId");

                    b.ToTable("SmartAgent");
                });

            modelBuilder.Entity("SmartDataHub.Models.InputMonitoring", b =>
                {
                    b.HasOne("SmartDataHub.Models.SmartAgent", "SmartAgent")
                        .WithMany()
                        .HasForeignKey("SmartAgentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmartDataHub.Models.Machine", b =>
                {
                    b.HasOne("SmartDataHub.Models.SmartAgent", "SmartAgent")
                        .WithMany()
                        .HasForeignKey("SmartAgentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmartDataHub.Models.MachineStateHistory", b =>
                {
                    b.HasOne("SmartDataHub.Models.Machine", "Machine")
                        .WithMany()
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
