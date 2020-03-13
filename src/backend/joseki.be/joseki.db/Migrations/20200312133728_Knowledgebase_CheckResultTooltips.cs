﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace joseki.db.Migrations
{
#pragma warning disable 1591
    public partial class Knowledgebase_CheckResultTooltips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Knowledgebase",
                columns: new[] { "Id", "ChangedBy", "Content", "DateCreated", "DateUpdated", "ItemId" },
                values: new object[,]
                {
                    { 46, null, "NoData means one of the following: 1. Joseki is not able to perform the Check due to not sufficient permissions; 2. Docker Image scanner was not able to complete a scan; 3. the Check requires a manual step to be performed", new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), "metadata.checks_nodata_description" },
                    { 47, null, "Warning indicates when Joseki found, likely, a not critical issue with a particular infrastructure component", new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), "metadata.checks_warning_description" },
                    { 48, null, "Failed check highlights the most critical issues that should be reviewed first", new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), "metadata.checks_failed_description" },
                    { 49, null, "You're good ;) A component satisfies verified rule", new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), "metadata.checks_passed_description" },
                    { 50, null, "The audit score indicates how close the infrastructure is to known best-practices configuration. The formula excludes NoData checks, gives doubled weight to Passed and Failed results: (Passed*2)/(Failed*2 + Passed*2 + Warning)", new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), new DateTime(2020, 3, 12, 13, 33, 48, 595, DateTimeKind.Utc).AddTicks(7551), "metadata.checks_score_description" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Knowledgebase",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Knowledgebase",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Knowledgebase",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Knowledgebase",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Knowledgebase",
                keyColumn: "Id",
                keyValue: 50);
        }
    }
#pragma warning restore 1591
}
