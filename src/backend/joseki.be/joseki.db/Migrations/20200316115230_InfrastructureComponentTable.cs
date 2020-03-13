﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace joseki.db.Migrations
{ 
#pragma warning disable 1591
    public partial class InfrastructureComponentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Audit_Date_ScannerId",
                table: "Audit");

            migrationBuilder.AlterColumn<string>(
                name: "ComponentId",
                table: "Audit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "undefined-id");

            migrationBuilder.AddColumn<int>(
                name: "InfrastructureComponentId",
                table: "Audit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InfrastructureComponent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    ChangedBy = table.Column<string>(nullable: true),
                    ScannerId = table.Column<string>(nullable: false),
                    ComponentId = table.Column<string>(nullable: false),
                    ComponentName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfrastructureComponent", x => x.Id);
                }); 

            migrationBuilder.Sql(@"
INSERT INTO [dbo].[InfrastructureComponent] 
    ([DateUpdated], [DateCreated], [ChangedBy], [ScannerId], [ComponentId], [ComponentName])
SELECT GETDATE() as DateUpdated, GETDATE() as DateCreated, null as ChangedBy, ScannerId, ComponentId, ComponentName
  FROM [dbo].[Audit]
  INNER JOIN (SELECT MAX([Id]) AS LatestAuditId FROM [dbo].[Audit] GROUP BY ComponentId) ids ON ids.LatestAuditId = [dbo].[Audit].Id
");

            migrationBuilder.Sql(@"
UPDATE [dbo].[Audit]
SET 
    [dbo].[Audit].[InfrastructureComponentId] = [dbo].[InfrastructureComponent].[Id] 
FROM 
    [dbo].[Audit]
    INNER JOIN [dbo].[InfrastructureComponent] ON [dbo].[InfrastructureComponent].[ComponentId] = [dbo].[Audit].[ComponentId]
WHERE [dbo].[Audit].[InfrastructureComponentId] = 0
");

            migrationBuilder.DropColumn(
                name: "ComponentName",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "ScannerId",
                table: "Audit");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_InfrastructureComponentId",
                table: "Audit",
                column: "InfrastructureComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Date_ComponentId",
                table: "Audit",
                columns: new[] { "Date", "ComponentId" },
                unique: true)
                .Annotation("SqlServer:Include", new[] { "AuditId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_InfrastructureComponent_InfrastructureComponentId",
                table: "Audit",
                column: "InfrastructureComponentId",
                principalTable: "InfrastructureComponent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_InfrastructureComponent_InfrastructureComponentId",
                table: "Audit");

            migrationBuilder.DropTable(
                name: "InfrastructureComponent");

            migrationBuilder.DropIndex(
                name: "IX_Audit_InfrastructureComponentId",
                table: "Audit");

            migrationBuilder.DropIndex(
                name: "IX_Audit_Date_ComponentId",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "InfrastructureComponentId",
                table: "Audit");

            migrationBuilder.AlterColumn<string>(
                name: "ComponentId",
                table: "Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "undefined-id",
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "ComponentName",
                table: "Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "undefined-name");

            migrationBuilder.AddColumn<string>(
                name: "ScannerId",
                table: "Audit",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Date_ScannerId",
                table: "Audit",
                columns: new[] { "Date", "ScannerId" },
                unique: true)
                .Annotation("SqlServer:Include", new[] { "AuditId" });
        }
    } 
#pragma warning restore 1591
}
