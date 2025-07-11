﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UsersTasks.API.Data;

#nullable disable

namespace UsersTasks.API.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20250621214538_AddTaskAuditEntity")]
    partial class AddTaskAuditEntity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AuditDemo.API.Data.Models.Audit.AppTaskAudit", b =>
                {
                    b.Property<int>("AuditId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AuditId"));

                    b.Property<int>("AuditOperation")
                        .HasColumnType("int");

                    b.Property<DateTime>("AuditedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("EndingState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousState")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuditId");

                    b.ToTable("AppTaskAudit", "audit");
                });

            modelBuilder.Entity("AuditDemo.API.Data.Models.Audit.UserAudit", b =>
                {
                    b.Property<int>("AuditId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AuditId"));

                    b.Property<int>("AuditOperation")
                        .HasColumnType("int");

                    b.Property<DateTime>("AuditedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("EndingState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreviousState")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuditId");

                    b.ToTable("UserAudit", "audit");
                });

            modelBuilder.Entity("UsersTasks.API.Data.Models.AppTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AppTasks");
                });

            modelBuilder.Entity("UsersTasks.API.Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AuditDemo.API.Data.Models.Audit.AppTaskAudit", b =>
                {
                    b.OwnsOne("AuditDemo.API.Data.Models.Audit.AppTaskSnapshot", "Entity", b1 =>
                        {
                            b1.Property<int>("AppTaskAuditAuditId")
                                .HasColumnType("int");

                            b1.Property<DateTime?>("CompletedAt")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Description")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<bool>("IsCompleted")
                                .HasColumnType("bit");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.Property<int>("Status")
                                .HasColumnType("int");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int?>("UserId")
                                .HasColumnType("int");

                            b1.HasKey("AppTaskAuditAuditId");

                            b1.HasIndex("UserId");

                            b1.ToTable("AppTaskAudit", "audit");

                            b1.WithOwner()
                                .HasForeignKey("AppTaskAuditAuditId");

                            b1.HasOne("UsersTasks.API.Data.Models.User", "User")
                                .WithMany()
                                .HasForeignKey("UserId");

                            b1.Navigation("User");
                        });

                    b.Navigation("Entity")
                        .IsRequired();
                });

            modelBuilder.Entity("AuditDemo.API.Data.Models.Audit.UserAudit", b =>
                {
                    b.OwnsOne("AuditDemo.API.Data.Models.Audit.UserSnapshot", "Entity", b1 =>
                        {
                            b1.Property<int>("UserAuditAuditId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UserAuditAuditId");

                            b1.ToTable("UserAudit", "audit");

                            b1.WithOwner()
                                .HasForeignKey("UserAuditAuditId");
                        });

                    b.Navigation("Entity")
                        .IsRequired();
                });

            modelBuilder.Entity("UsersTasks.API.Data.Models.AppTask", b =>
                {
                    b.HasOne("UsersTasks.API.Data.Models.User", "User")
                        .WithMany("Tasks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("UsersTasks.API.Data.Models.User", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
