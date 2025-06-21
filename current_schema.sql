IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241128131039_init', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [DisplayName] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241130180611_add appuser', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [FurnitureBrands] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_FurnitureBrands] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FurnitureCategories] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_FurnitureCategories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Furniture] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [PictureUrl] nvarchar(max) NOT NULL,
    [FurnitureModel] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [BrandId] uniqueidentifier NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Furniture] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Furniture_FurnitureBrands_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [FurnitureBrands] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Furniture_FurnitureCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [FurnitureCategories] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Furniture_BrandId] ON [Furniture] ([BrandId]);
GO

CREATE INDEX [IX_Furniture_CategoryId] ON [Furniture] ([CategoryId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241130183137_add Furniture entity', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Furniture]') AND [c].[name] = N'Name');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Furniture] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Furniture] ALTER COLUMN [Name] nvarchar(100) NOT NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Furniture]') AND [c].[name] = N'Description');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Furniture] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Furniture] ALTER COLUMN [Description] nvarchar(180) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241130185346_entity2', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Address] (
    [Id] uniqueidentifier NOT NULL,
    [FName] nvarchar(max) NOT NULL,
    [LName] nvarchar(max) NOT NULL,
    [Street] nvarchar(max) NOT NULL,
    [City] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Country] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Address_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Address_UserId] ON [Address] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250216173505_AddAddressTable', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Carts] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Carts_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CartItems] (
    [Id] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL DEFAULT 1,
    [CartId] uniqueidentifier NOT NULL,
    [FurnitureId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartItems_Furniture_FurnitureId] FOREIGN KEY ([FurnitureId]) REFERENCES [Furniture] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_CartItems_CartId_FurnitureId] ON [CartItems] ([CartId], [FurnitureId]);
GO

CREATE INDEX [IX_CartItems_FurnitureId] ON [CartItems] ([FurnitureId]);
GO

CREATE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250305004255_cart module', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Furniture] ADD [DiscountPercentage] decimal(18,2) NULL;
GO

ALTER TABLE [Furniture] ADD [DiscountedPrice] decimal(18,2) NULL;
GO

ALTER TABLE [Furniture] ADD [HasOffer] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Furniture] ADD [OfferEndDate] datetime2 NULL;
GO

ALTER TABLE [Furniture] ADD [OfferStartDate] datetime2 NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250308223813_add Offer', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [CartItems] ADD [CartId1] uniqueidentifier NULL;
GO

CREATE INDEX [IX_CartItems_CartId1] ON [CartItems] ([CartId1]);
GO

ALTER TABLE [CartItems] ADD CONSTRAINT [FK_CartItems_Carts_CartId1] FOREIGN KEY ([CartId1]) REFERENCES [Carts] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250315215555_items', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [CartItems] DROP CONSTRAINT [FK_CartItems_Carts_CartId1];
GO

DROP INDEX [IX_Carts_UserId] ON [Carts];
GO

DROP INDEX [IX_CartItems_CartId1] ON [CartItems];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CartItems]') AND [c].[name] = N'CartId1');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [CartItems] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [CartItems] DROP COLUMN [CartId1];
GO

CREATE UNIQUE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250316032906_cart items', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Orders] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CartId] uniqueidentifier NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [OrderAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id])
);
GO

CREATE TABLE [OrdersItems] (
    [Id] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [CartId] uniqueidentifier NOT NULL,
    [FurnitureId] uniqueidentifier NOT NULL,
    [OrderId] uniqueidentifier NULL,
    CONSTRAINT [PK_OrdersItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrdersItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrdersItems_Furniture_FurnitureId] FOREIGN KEY ([FurnitureId]) REFERENCES [Furniture] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrdersItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250502135711_AddingOrderModel', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [NotificationPreferences] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [EmailNotifications] bit NOT NULL DEFAULT CAST(1 AS bit),
    [OrderUpdates] bit NOT NULL DEFAULT CAST(1 AS bit),
    [PromotionalOffers] bit NOT NULL DEFAULT CAST(1 AS bit),
    [SystemAlerts] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CartReminders] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_NotificationPreferences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_NotificationPreferences_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Notifications] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [Type] int NOT NULL,
    [IsRead] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [ReadAt] datetime2 NULL,
    [RelatedEntityId] nvarchar(max) NULL,
    [RelatedEntityType] nvarchar(max) NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_NotificationPreferences_UserId] ON [NotificationPreferences] ([UserId]);
GO

CREATE INDEX [IX_Notifications_CreatedAt] ON [Notifications] ([CreatedAt]);
GO

CREATE INDEX [IX_Notifications_IsRead] ON [Notifications] ([IsRead]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_Notifications_UserId_IsRead] ON [Notifications] ([UserId], [IsRead]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250526024717_AddNotificationModule', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [RefreshTokens] (
    [Id] uniqueidentifier NOT NULL,
    [Token] nvarchar(max) NOT NULL,
    [Expires] datetime2 NOT NULL,
    [Created] datetime2 NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [JwtId] nvarchar(max) NOT NULL,
    [IsRevoked] bit NOT NULL,
    [ReplacedByToken] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529065116_AddRefreshTokens', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Ratings] (
    [Id] uniqueidentifier NOT NULL,
    [Stars] int NOT NULL,
    [Review] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Ratings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ratings_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ratings_Furniture_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Furniture] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Ratings_ProductId] ON [Ratings] ([ProductId]);
GO

CREATE INDEX [IX_Ratings_UserId] ON [Ratings] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250609155607_RatingModule', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Furniture] ADD [ColorId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

ALTER TABLE [Furniture] ADD [StyleTypeId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

CREATE TABLE [Colors] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Hex] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Colors] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StyleTypes] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StyleTypes] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_Furniture_ColorId] ON [Furniture] ([ColorId]);
GO

CREATE INDEX [IX_Furniture_StyleTypeId] ON [Furniture] ([StyleTypeId]);
GO

ALTER TABLE [Furniture] ADD CONSTRAINT [FK_Furniture_Colors_ColorId] FOREIGN KEY ([ColorId]) REFERENCES [Colors] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Furniture] ADD CONSTRAINT [FK_Furniture_StyleTypes_StyleTypeId] FOREIGN KEY ([StyleTypeId]) REFERENCES [StyleTypes] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250616001122_addStyleTypesAndColorProperties', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Furniture]') AND [c].[name] = N'Description');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Furniture] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Furniture] ALTER COLUMN [Description] nvarchar(400) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250616150736_IncreaseDescriptionLengthOnly', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Carts_CartId];
GO

ALTER TABLE [OrdersItems] DROP CONSTRAINT [FK_OrdersItems_Orders_OrderId];
GO

ALTER TABLE [Carts] ADD [DeliveryMethodId] uniqueidentifier NULL;
GO

ALTER TABLE [Carts] ADD [PaymentIntentId] nvarchar(max) NOT NULL DEFAULT N'';
GO

CREATE TABLE [DeliveryMethods] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Cost] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_DeliveryMethods] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_Orders_OrderDate] ON [Orders] ([OrderDate]);
GO

CREATE INDEX [IX_Carts_DeliveryMethodId] ON [Carts] ([DeliveryMethodId]);
GO

ALTER TABLE [Carts] ADD CONSTRAINT [FK_Carts_DeliveryMethods_DeliveryMethodId] FOREIGN KEY ([DeliveryMethodId]) REFERENCES [DeliveryMethods] ([Id]);
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]);
GO

ALTER TABLE [OrdersItems] ADD CONSTRAINT [FK_OrdersItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250618165400_addDeliveryMethod', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Carts]') AND [c].[name] = N'PaymentIntentId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Carts] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Carts] ALTER COLUMN [PaymentIntentId] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250620211253_MakePaymentIntentIdNullable', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250620233410_add', N'8.0.11');
GO

COMMIT;
GO

