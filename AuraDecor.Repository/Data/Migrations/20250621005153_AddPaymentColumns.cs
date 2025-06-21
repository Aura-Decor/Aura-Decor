using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDecor.Repositoriy.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to conditionally add columns only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'DeliveryMethodId')
                BEGIN
                    ALTER TABLE [Orders] ADD [DeliveryMethodId] uniqueidentifier NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'PaymentIntentId')
                BEGIN
                    ALTER TABLE [Orders] ADD [PaymentIntentId] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'PaymentStatus')
                BEGIN
                    ALTER TABLE [Orders] ADD [PaymentStatus] int NOT NULL DEFAULT 0;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'ShippingAddressId')
                BEGIN
                    ALTER TABLE [Orders] ADD [ShippingAddressId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                END
            ");

            // Create indexes only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_DeliveryMethodId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE INDEX [IX_Orders_DeliveryMethodId] ON [Orders] ([DeliveryMethodId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_ShippingAddressId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE INDEX [IX_Orders_ShippingAddressId] ON [Orders] ([ShippingAddressId]);
                END
            ");

            // Add foreign keys only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Address_ShippingAddressId')
                BEGIN
                    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Address_ShippingAddressId] 
                    FOREIGN KEY ([ShippingAddressId]) REFERENCES [Address] ([Id]) ON DELETE CASCADE;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_DeliveryMethods_DeliveryMethodId')
                BEGIN
                    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_DeliveryMethods_DeliveryMethodId] 
                    FOREIGN KEY ([DeliveryMethodId]) REFERENCES [DeliveryMethods] ([Id]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Address_ShippingAddressId')
                BEGIN
                    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Address_ShippingAddressId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_DeliveryMethods_DeliveryMethodId')
                BEGIN
                    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_DeliveryMethods_DeliveryMethodId];
                END
            ");

            // Drop indexes if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_DeliveryMethodId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    DROP INDEX [IX_Orders_DeliveryMethodId] ON [Orders];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_ShippingAddressId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    DROP INDEX [IX_Orders_ShippingAddressId] ON [Orders];
                END
            ");

            // Drop columns if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'DeliveryMethodId')
                BEGIN
                    ALTER TABLE [Orders] DROP COLUMN [DeliveryMethodId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'PaymentIntentId')
                BEGIN
                    ALTER TABLE [Orders] DROP COLUMN [PaymentIntentId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'PaymentStatus')
                BEGIN
                    ALTER TABLE [Orders] DROP COLUMN [PaymentStatus];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'ShippingAddressId')
                BEGIN
                    ALTER TABLE [Orders] DROP COLUMN [ShippingAddressId];
                END
            ");
        }
    }
}
