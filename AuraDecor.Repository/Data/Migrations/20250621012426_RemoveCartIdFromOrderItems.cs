using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDecor.Repositoriy.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCartIdFromOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if foreign key exists before dropping
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrdersItems_Carts_CartId')
                BEGIN
                    ALTER TABLE [OrdersItems] DROP CONSTRAINT [FK_OrdersItems_Carts_CartId]
                END
            ");

            // Check if index exists before dropping
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdersItems_CartId' AND object_id = OBJECT_ID('OrdersItems'))
                BEGIN
                    DROP INDEX [IX_OrdersItems_CartId] ON [OrdersItems]
                END
            ");

            // Check if column exists before dropping
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdersItems') AND name = 'CartId')
                BEGIN
                    ALTER TABLE [OrdersItems] DROP COLUMN [CartId]
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CartId",
                table: "OrdersItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrdersItems_CartId",
                table: "OrdersItems",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersItems_Carts_CartId",
                table: "OrdersItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
