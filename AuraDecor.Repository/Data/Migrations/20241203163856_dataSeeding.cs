using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDecor.Repositoriy.Migrations
{
    /// <inheritdoc />
    public partial class dataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furniture_FurnitureBrands_BrandId",
                table: "Furniture");

            migrationBuilder.DropForeignKey(
                name: "FK_Furniture_FurnitureCategories_CategoryId",
                table: "Furniture");

            migrationBuilder.AddForeignKey(
                name: "FK_Furniture_FurnitureBrands_BrandId",
                table: "Furniture",
                column: "BrandId",
                principalTable: "FurnitureBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Furniture_FurnitureCategories_CategoryId",
                table: "Furniture",
                column: "CategoryId",
                principalTable: "FurnitureCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furniture_FurnitureBrands_BrandId",
                table: "Furniture");

            migrationBuilder.DropForeignKey(
                name: "FK_Furniture_FurnitureCategories_CategoryId",
                table: "Furniture");

            migrationBuilder.AddForeignKey(
                name: "FK_Furniture_FurnitureBrands_BrandId",
                table: "Furniture",
                column: "BrandId",
                principalTable: "FurnitureBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Furniture_FurnitureCategories_CategoryId",
                table: "Furniture",
                column: "CategoryId",
                principalTable: "FurnitureCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
