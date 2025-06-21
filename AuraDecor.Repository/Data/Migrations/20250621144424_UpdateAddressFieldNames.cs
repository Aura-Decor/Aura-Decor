using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDecor.Repositoriy.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddressFieldNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_UserId",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_UserId",
                table: "Address");

            // Rename existing columns to preserve data
            migrationBuilder.RenameColumn(
                name: "FName",
                table: "Address",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "LName",
                table: "Address",
                newName: "LastName");

            // Make UserId nullable
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Address",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // Add new columns
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryInstructions",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                table: "Address",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_UserId",
                table: "Address",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_UserId",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_UserId",
                table: "Address");

            // Drop the new columns
            migrationBuilder.DropColumn(
                name: "State",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "DeliveryInstructions",
                table: "Address");

            // Rename columns back to original names
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Address",
                newName: "FName");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Address",
                newName: "LName");

            // Make UserId required again
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Address",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                table: "Address",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_UserId",
                table: "Address",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
