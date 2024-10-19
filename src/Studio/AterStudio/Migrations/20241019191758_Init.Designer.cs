﻿// <auto-generated />
using System;
using EntityFramework.DBProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AterStudio.Migrations
{
    [DbContext(typeof(CommandDbContext))]
    [Migration("20241019191758_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1");

            modelBuilder.Entity("Entity.ApiDocInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocalPath")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ApiDocInfos");
                });

            modelBuilder.Entity("Entity.ConfigData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<int>("ValueType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("Entity.EntityInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AssemblyName")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasMaxLength(300)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsEnum")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsList")
                        .HasColumnType("INTEGER");

                    b.Property<int>("KeyType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Md5Hash")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("ModuleName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("NamespaceName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Summary")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("ProjectId");

                    b.ToTable("EntityInfos");
                });

            modelBuilder.Entity("Entity.GenAction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("EntityPath")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<string>("OpenApiPath")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SourceType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Description");

                    b.HasIndex("Name");

                    b.HasIndex("ProjectId");

                    b.ToTable("GenActions");
                });

            modelBuilder.Entity("Entity.GenStep", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasMaxLength(100000)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("GenStepType")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OutputContent")
                        .HasMaxLength(100000)
                        .HasColumnType("TEXT");

                    b.Property<string>("OutputPath")
                        .HasMaxLength(400)
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasMaxLength(400)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("GenSteps");
                });

            modelBuilder.Entity("Entity.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int?>("SolutionType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Entity.PropertyInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AttributeText")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("CommentSummary")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("CommentXml")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DefaultValue")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EntityInfoId")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("HasMany")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasSet")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsComplexType")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDecimal")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsEnum")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsJsonIgnore")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsList")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsNavigation")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsNullable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MaxLength")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MinLength")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("NavigationName")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("SuffixContent")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EntityInfoId");

                    b.HasIndex("IsEnum");

                    b.HasIndex("Name");

                    b.HasIndex("Type");

                    b.ToTable("PropertyInfo");
                });

            modelBuilder.Entity("GenActionGenStep", b =>
                {
                    b.Property<Guid>("GenActionsId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GenStepsId")
                        .HasColumnType("TEXT");

                    b.HasKey("GenActionsId", "GenStepsId");

                    b.HasIndex("GenStepsId");

                    b.ToTable("GenActionGenStep");
                });

            modelBuilder.Entity("Entity.ApiDocInfo", b =>
                {
                    b.HasOne("Entity.Project", "Project")
                        .WithMany("ApiDocInfos")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Entity.EntityInfo", b =>
                {
                    b.HasOne("Entity.Project", "Project")
                        .WithMany("EntityInfos")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Entity.GenAction", b =>
                {
                    b.HasOne("Entity.Project", "Project")
                        .WithMany("GenActions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Entity.Variable", "Variables", b1 =>
                        {
                            b1.Property<Guid>("GenActionId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("__synthesizedOrdinal")
                                .ValueGeneratedOnAddOrUpdate()
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Key")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("TEXT");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("TEXT");

                            b1.HasKey("GenActionId", "__synthesizedOrdinal");

                            b1.ToTable("GenActions");

                            b1.ToJson("Variables");

                            b1.WithOwner()
                                .HasForeignKey("GenActionId");
                        });

                    b.Navigation("Project");

                    b.Navigation("Variables");
                });

            modelBuilder.Entity("Entity.GenStep", b =>
                {
                    b.HasOne("Entity.Project", "Project")
                        .WithMany("GenSteps")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Entity.Project", b =>
                {
                    b.OwnsOne("Entity.ProjectConfig", "Config", b1 =>
                        {
                            b1.Property<Guid>("ProjectId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("ApiPath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("ApplicationPath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("ControllerType")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("CreatedTimeName")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("EntityFrameworkPath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("EntityPath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("IdType")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<bool>("IsLight")
                                .HasColumnType("INTEGER");

                            b1.Property<bool?>("IsSplitController")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("MicroservicePath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("SharePath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("SolutionPath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("UpdatedTimeName")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Version")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("ProjectId");

                            b1.ToTable("Projects");

                            b1.ToJson("Config");

                            b1.WithOwner()
                                .HasForeignKey("ProjectId");
                        });

                    b.Navigation("Config")
                        .IsRequired();
                });

            modelBuilder.Entity("Entity.PropertyInfo", b =>
                {
                    b.HasOne("Entity.EntityInfo", "EntityInfo")
                        .WithMany("PropertyInfos")
                        .HasForeignKey("EntityInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntityInfo");
                });

            modelBuilder.Entity("GenActionGenStep", b =>
                {
                    b.HasOne("Entity.GenAction", null)
                        .WithMany()
                        .HasForeignKey("GenActionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entity.GenStep", null)
                        .WithMany()
                        .HasForeignKey("GenStepsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entity.EntityInfo", b =>
                {
                    b.Navigation("PropertyInfos");
                });

            modelBuilder.Entity("Entity.Project", b =>
                {
                    b.Navigation("ApiDocInfos");

                    b.Navigation("EntityInfos");

                    b.Navigation("GenActions");

                    b.Navigation("GenSteps");
                });
#pragma warning restore 612, 618
        }
    }
}
