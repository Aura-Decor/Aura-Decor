using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDecor.Repositoriy.Migrations
{
    /// <inheritdoc />
    public partial class addStyleTypesAndColorProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Color",
            //    table: "Furniture");

            //migrationBuilder.DropColumn(
            //    name: "Style",
            //    table: "Furniture");

            migrationBuilder.AddColumn<Guid>(
                name: "ColorId",
                table: "Furniture",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StyleTypeId",
                table: "Furniture",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hex = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StyleTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StyleTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Furniture_ColorId",
                table: "Furniture",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Furniture_StyleTypeId",
                table: "Furniture",
                column: "StyleTypeId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Furniture_Colors_ColorId",
            //    table: "Furniture",
            //    column: "ColorId",
            //    principalTable: "Colors",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Furniture_StyleTypes_StyleTypeId",
            //    table: "Furniture",
            //    column: "StyleTypeId",
            //    principalTable: "StyleTypes",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Furniture_Colors_ColorId",
                table: "Furniture");

            migrationBuilder.DropForeignKey(
                name: "FK_Furniture_StyleTypes_StyleTypeId",
                table: "Furniture");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "StyleTypes");

            migrationBuilder.DropIndex(
                name: "IX_Furniture_ColorId",
                table: "Furniture");

            migrationBuilder.DropIndex(
                name: "IX_Furniture_StyleTypeId",
                table: "Furniture");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "Furniture");

            migrationBuilder.DropColumn(
                name: "StyleTypeId",
                table: "Furniture");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Furniture",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Style",
                table: "Furniture",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
