﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ProjectMaestro.EngineCore;

#nullable disable

namespace ProjectMaestro.EngineCore.Migrations
{
    [DbContext(typeof(WorkflowEngineDbContext))]
    partial class WorkflowEngineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ProjectMaestro.EngineCore.TaskDefinition", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.PrimitiveCollection<List<string>>("NextTasks")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<bool?>("Root")
                        .HasColumnType("boolean");

                    b.Property<string>("WorkflowDefinitionId")
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.HasIndex("WorkflowDefinitionId");

                    b.ToTable("TaskDefinitions");
                });

            modelBuilder.Entity("ProjectMaestro.EngineCore.TaskInstance", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WorkflowId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TaskInstances");
                });

            modelBuilder.Entity("ProjectMaestro.EngineCore.WorkflowDefinition", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WorkflowDefinitions");
                });

            modelBuilder.Entity("ProjectMaestro.EngineCore.WorkflowInstance", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CurrentTaskId")
                        .HasColumnType("text");

                    b.Property<string>("DefinitionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WorkflowInstances");
                });

            modelBuilder.Entity("ProjectMaestro.EngineCore.TaskDefinition", b =>
                {
                    b.HasOne("ProjectMaestro.EngineCore.WorkflowDefinition", null)
                        .WithMany("Tasks")
                        .HasForeignKey("WorkflowDefinitionId");
                });

            modelBuilder.Entity("ProjectMaestro.EngineCore.WorkflowDefinition", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
