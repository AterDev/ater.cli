﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AterStudio.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ValueType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    SolutionType = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Config = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiDocInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    LocalPath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDocInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiDocInfos_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Md5Hash = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ModuleName = table.Column<string>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NamespaceName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AssemblyName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    KeyType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnum = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsList = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityInfos_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SourceType = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenActions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateContent = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    Command = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    GenStepType = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenSteps_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsList = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsNavigation = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsJsonIgnore = table.Column<bool>(type: "INTEGER", nullable: false),
                    NavigationName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsComplexType = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasMany = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsEnum = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasSet = table.Column<bool>(type: "INTEGER", nullable: false),
                    AttributeText = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CommentXml = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CommentSummary = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsNullable = table.Column<bool>(type: "INTEGER", nullable: false),
                    MinLength = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxLength = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDecimal = table.Column<bool>(type: "INTEGER", nullable: false),
                    SuffixContent = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedTime = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyInfo_EntityInfos_EntityInfoId",
                        column: x => x.EntityInfoId,
                        principalTable: "EntityInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenActionGenStep",
                columns: table => new
                {
                    GenActionsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GenStepsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenActionGenStep", x => new { x.GenActionsId, x.GenStepsId });
                    table.ForeignKey(
                        name: "FK_GenActionGenStep_GenActions_GenActionsId",
                        column: x => x.GenActionsId,
                        principalTable: "GenActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenActionGenStep_GenSteps_GenStepsId",
                        column: x => x.GenStepsId,
                        principalTable: "GenSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocInfos_ProjectId",
                table: "ApiDocInfos",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityInfos_Name",
                table: "EntityInfos",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EntityInfos_ProjectId",
                table: "EntityInfos",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GenActionGenStep_GenStepsId",
                table: "GenActionGenStep",
                column: "GenStepsId");

            migrationBuilder.CreateIndex(
                name: "IX_GenActions_ProjectId",
                table: "GenActions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GenSteps_ProjectId",
                table: "GenSteps",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInfo_EntityInfoId",
                table: "PropertyInfo",
                column: "EntityInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInfo_IsEnum",
                table: "PropertyInfo",
                column: "IsEnum");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInfo_Name",
                table: "PropertyInfo",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInfo_Type",
                table: "PropertyInfo",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiDocInfos");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "GenActionGenStep");

            migrationBuilder.DropTable(
                name: "PropertyInfo");

            migrationBuilder.DropTable(
                name: "GenActions");

            migrationBuilder.DropTable(
                name: "GenSteps");

            migrationBuilder.DropTable(
                name: "EntityInfos");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
