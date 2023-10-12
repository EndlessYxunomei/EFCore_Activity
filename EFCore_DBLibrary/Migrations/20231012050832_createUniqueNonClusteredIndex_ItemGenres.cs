using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore_DBLibrary.Migrations
{
    /// <inheritdoc />
    public partial class createUniqueNonClusteredIndex_ItemGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemGenres_ItemId",
                table: "ItemGenres");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGenres_ItemId_GenreId",
                table: "ItemGenres",
                columns: new[] { "ItemId", "GenreId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemGenres_ItemId_GenreId",
                table: "ItemGenres");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGenres_ItemId",
                table: "ItemGenres",
                column: "ItemId");
        }
    }
}
