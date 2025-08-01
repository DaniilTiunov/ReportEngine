﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ReportEngine.Domain.Database.Context;

#nullable disable

namespace ReportEngine.Domain.Migrations
{
    [DbContext(typeof(ReAppContext))]
    [Migration("20250731041137_UpdateTables")]
    partial class UpdateTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ReportEngine.Domain.Entities.Armautre.CarbonArmature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("CarbonArmatures");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Armautre.HeaterArmature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("HeaterArmatures");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Armautre.StainlessArmature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("StainlessArmatures");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Braces.BoxesBrace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BoxesBraces");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Braces.DrainageBrace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DrainageBraces");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Braces.SensorBrace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SensorsBraces");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Drainage.Drainage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Drainages");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricComponents.CabelBoxe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Cabel")
                        .HasColumnType("integer");

                    b.Property<int>("CabelInput")
                        .HasColumnType("integer");

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ElectricProtection")
                        .HasColumnType("integer");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CabelBoxes");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricComponents.CabelInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Cabel")
                        .HasColumnType("integer");

                    b.Property<int>("CabelInput")
                        .HasColumnType("integer");

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ElectricProtection")
                        .HasColumnType("integer");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CabelInputs");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricComponents.CabelProduction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Cabel")
                        .HasColumnType("integer");

                    b.Property<int>("CabelInput")
                        .HasColumnType("integer");

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ElectricProtection")
                        .HasColumnType("integer");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CabelProductions");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricComponents.CabelProtection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Cabel")
                        .HasColumnType("integer");

                    b.Property<int>("CabelInput")
                        .HasColumnType("integer");

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ElectricProtection")
                        .HasColumnType("integer");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CabelProtections");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricComponents.Heater", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Cabel")
                        .HasColumnType("integer");

                    b.Property<int>("CabelInput")
                        .HasColumnType("integer");

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ElectricProtection")
                        .HasColumnType("integer");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Heaters");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricSockets.CarbonSocket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("CarbonSockets");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricSockets.HeaterSocket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("HeaterSockets");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ElectricSockets.StainlessSocket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("StainlessSockets");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Frame.FrameDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FrameDetails");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Frame.FrameRoll", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FrameRolls");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Frame.PillarEqiup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PillarEqiups");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Obvyazka", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Clamp")
                        .HasColumnType("real");

                    b.Property<float>("HumanCost")
                        .HasColumnType("real");

                    b.Property<float>("LineLength")
                        .HasColumnType("real");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("OtherLineCount")
                        .HasColumnType("integer");

                    b.Property<int>("Sensor")
                        .HasColumnType("integer");

                    b.Property<string>("SensorType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("TreeSocket")
                        .HasColumnType("real");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("WidthOnFrame")
                        .HasColumnType("real");

                    b.Property<float>("ZraCount")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Obvyazki");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Other.Container", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Containers");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Other.Other", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Others");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Pipes.CarbonPipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("CarbonPipes");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Pipes.HeaterPipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("HeaterPipes");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Pipes.StainlessPipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<float>("Depth")
                        .HasColumnType("real");

                    b.Property<int>("ExportDays")
                        .HasColumnType("integer");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("StainlessPipes");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ProjectInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Company")
                        .HasColumnType("text");

                    b.Property<decimal>("Cost")
                        .HasColumnType("numeric");

                    b.Property<DateOnly>("CreationDate")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsGalvanized")
                        .HasColumnType("boolean");

                    b.Property<string>("MarkMinus")
                        .HasColumnType("text");

                    b.Property<string>("MarkPlus")
                        .HasColumnType("text");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<string>("Object")
                        .HasColumnType("text");

                    b.Property<string>("OrderCustomer")
                        .HasColumnType("text");

                    b.Property<DateOnly>("OutOfProduction")
                        .HasColumnType("date");

                    b.Property<string>("RequestProduction")
                        .HasColumnType("text");

                    b.Property<int>("StandCount")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Stand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Armature")
                        .HasColumnType("text");

                    b.Property<string>("BraceType")
                        .HasColumnType("text");

                    b.Property<string>("Design")
                        .HasColumnType("text");

                    b.Property<int>("Devices")
                        .HasColumnType("integer");

                    b.Property<string>("KKSCode")
                        .HasColumnType("text");

                    b.Property<string>("KMCH")
                        .HasColumnType("text");

                    b.Property<string>("MaterialLine")
                        .HasColumnType("text");

                    b.Property<int>("NN")
                        .HasColumnType("integer");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("ObvyazkaType")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectInfoId")
                        .HasColumnType("integer");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("text");

                    b.Property<decimal>("StandSummCost")
                        .HasColumnType("numeric");

                    b.Property<string>("TreeScoket")
                        .HasColumnType("text");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ProjectInfoId");

                    b.ToTable("Stands");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Cabinet")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PhoneContact")
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .HasColumnType("text");

                    b.Property<string>("SecondName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.Stand", b =>
                {
                    b.HasOne("ReportEngine.Domain.Entities.ProjectInfo", "Project")
                        .WithMany("Stands")
                        .HasForeignKey("ProjectInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ReportEngine.Domain.Entities.ProjectInfo", b =>
                {
                    b.Navigation("Stands");
                });
#pragma warning restore 612, 618
        }
    }
}
