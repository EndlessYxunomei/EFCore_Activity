using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EFCore_DBLibrary.Migrations
{
    /// <inheritdoc />
    public partial class dataUpdate_SeedGenresMigrationCategoriesInInvetoryMigrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "CreatedByUserId", "CreatedDate", "IsActive", "IsDeleted", "LastModifiedDate", "LastModifiedUserId", "Name" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, null, "Fantasy" },
                    { 2, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, null, "Sci/Fi" },
                    { 3, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, null, "Horror" },
                    { 4, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, null, "Comedy" },
                    { 5, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, null, "Drama" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
