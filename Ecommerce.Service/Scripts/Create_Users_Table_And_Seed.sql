-- =============================================
-- Script: Create Users Table and Seed Default Users
-- Database: ProductCatalog
-- =============================================

USE [ProductCatalog];
GO

-- 1. Create Users Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL CONSTRAINT UQ_Users_Username UNIQUE,
        [Email] NVARCHAR(100) NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [PasswordSalt] NVARCHAR(MAX) NOT NULL,
        [Role] NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_Role DEFAULT 'User',
        [FullName] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT GETUTCDATE()
    );
    PRINT 'Table [dbo].[Users] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Users] already exists.';
END
GO

-- 2. Index on Username for fast lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Users_Username ON [dbo].[Users]([Username]);
    PRINT 'Index IX_Users_Username created.';
END
GO

-- 3. Seed Default Admin and User
-- Admin Credentials:
--   Username: admin
--   Password: Admin@123
--   Role: Admin
-- User Credentials:
--   Username: user
--   Password: User@123
--   Role: User
-- (Passwords are hashed using HMAC-SHA512)

IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [PasswordSalt], [Role], [FullName], [IsActive], [CreatedAt])
    VALUES (
        'admin',
        'admin@ecommerce.com',
        'S0dy9W/IUBcgL1sV0u0JwJOiZkxxZfTVb75KMVOcuHC1qxLMC41O4axa8u4ehN0MJIPtVLfPJKcXpVeYDc5J8w==',
        'GIRURD+D8VG4Av1UG/I2tq1z5Cvcc+lP71sJGen0oWVeBcLSAiQob5uj/WQwTMYEr5Apf90quRGcD0G/uo8de4Ct9W3VvJG4NUsQba/DzYTGA9AWZ76X1MI9V25GzRFqkyjRyzfiUXjNy9aKfRefRJfqTKD1BuTpTOXHBTmLx6w=',
        'Admin',
        'System Administrator',
        1,
        GETUTCDATE()
    );
    PRINT 'Admin user seeded.';
END
ELSE
BEGIN
    UPDATE [dbo].[Users]
    SET [PasswordHash] = 'S0dy9W/IUBcgL1sV0u0JwJOiZkxxZfTVb75KMVOcuHC1qxLMC41O4axa8u4ehN0MJIPtVLfPJKcXpVeYDc5J8w==',
        [PasswordSalt] = 'GIRURD+D8VG4Av1UG/I2tq1z5Cvcc+lP71sJGen0oWVeBcLSAiQob5uj/WQwTMYEr5Apf90quRGcD0G/uo8de4Ct9W3VvJG4NUsQba/DzYTGA9AWZ76X1MI9V25GzRFqkyjRyzfiUXjNy9aKfRefRJfqTKD1BuTpTOXHBTmLx6w='
    WHERE [Username] = 'admin';
    PRINT 'Admin user password updated.';
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'user')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [PasswordSalt], [Role], [FullName], [IsActive], [CreatedAt])
    VALUES (
        'user',
        'user@ecommerce.com',
        'vaZlmB03RGePSP8B29Hrj9P9UjrYahw80LPVSckfywZT70Ret0ti4gaCooNOytVRm4Hys280VHcRrBUeORN/uQ==',
        'Q5FKm0Wos1Je0L9uXqlgcyhFqQAlZ7vKIuQBJqlxPt2CPralHyIvtVHiHXhM+nFWnJ1Vbpgby9SS1ohGXmhS9ZvjpAEIL7F1jWYgOSHHJri0WT3Gw3kfiUkg7JIUstSmWS3pPXnBHD3BS2ULzpmXHpwtNwbsp3xbHLWL9RbPG40=',
        'User',
        'Normal Customer',
        1,
        GETUTCDATE()
    );
    PRINT 'Standard user seeded.';
END
ELSE
BEGIN
    UPDATE [dbo].[Users]
    SET [PasswordHash] = 'vaZlmB03RGePSP8B29Hrj9P9UjrYahw80LPVSckfywZT70Ret0ti4gaCooNOytVRm4Hys280VHcRrBUeORN/uQ==',
        [PasswordSalt] = 'Q5FKm0Wos1Je0L9uXqlgcyhFqQAlZ7vKIuQBJqlxPt2CPralHyIvtVHiHXhM+nFWnJ1Vbpgby9SS1ohGXmhS9ZvjpAEIL7F1jWYgOSHHJri0WT3Gw3kfiUkg7JIUstSmWS3pPXnBHD3BS2ULzpmXHpwtNwbsp3xbHLWL9RbPG40='
    WHERE [Username] = 'user';
    PRINT 'Standard user password updated.';
END
GO
