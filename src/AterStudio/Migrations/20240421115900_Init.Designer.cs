﻿// <auto-generated />
using System;
using Definition.EntityFramework.DBProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AterStudio.Migrations
{
    [DbContext(typeof(ContextBase))]
    [Migration("20240421115900_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Definition.Entity.ApiDocInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

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

            modelBuilder.Entity("Definition.Entity.ConfigData", b =>
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

                    b.Property<Guid>("ProjectId")
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

                    b.HasIndex("ProjectId");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("Definition.Entity.EntityInfo", b =>
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

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsEnum")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsList")
                        .HasColumnType("INTEGER");

                    b.Property<int>("KeyType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("NamespaceName")
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

            modelBuilder.Entity("Definition.Entity.Project", b =>
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

            modelBuilder.Entity("Definition.Entity.PropertyInfo", b =>
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

            modelBuilder.Entity("Definition.Entity.TemplateFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasMaxLength(10000)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("TemplateFiles");
                });

            modelBuilder.Entity("Definition.Entity.ApiDocInfo", b =>
                {
                    b.HasOne("Definition.Entity.Project", "Project")
                        .WithMany("ApiDocInfos")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Definition.Entity.ConfigData", b =>
                {
                    b.HasOne("Definition.Entity.Project", "Project")
                        .WithMany("ConfigDatas")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Definition.Entity.EntityInfo", b =>
                {
                    b.HasOne("Definition.Entity.Project", "Project")
                        .WithMany("EntityInfos")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Definition.Entity.PropertyInfo", b =>
                {
                    b.HasOne("Definition.Entity.EntityInfo", "EntityInfo")
                        .WithMany("PropertyInfos")
                        .HasForeignKey("EntityInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntityInfo");
                });

            modelBuilder.Entity("Definition.Entity.TemplateFile", b =>
                {
                    b.HasOne("Definition.Entity.Project", "Project")
                        .WithMany("TemplateFiles")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Definition.Entity.EntityInfo", b =>
                {
                    b.Navigation("PropertyInfos");
                });

            modelBuilder.Entity("Definition.Entity.Project", b =>
                {
                    b.Navigation("ApiDocInfos");

                    b.Navigation("ConfigDatas");

                    b.Navigation("EntityInfos");

                    b.Navigation("TemplateFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
