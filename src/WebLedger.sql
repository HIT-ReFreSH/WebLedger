CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Access` (
    `Name` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `Key` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Access` PRIMARY KEY (`Name`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Categories` (
    `Name` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `SuperCategoryName` varchar(64) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Categories` PRIMARY KEY (`Name`),
    CONSTRAINT `FK_Categories_Categories_SuperCategoryName` FOREIGN KEY (`SuperCategoryName`) REFERENCES `Categories` (`Name`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Configs` (
    `Key` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `Value` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Configs` PRIMARY KEY (`Key`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Types` (
    `Name` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `DefaultCategoryName` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `DefaultIsIncome` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Types` PRIMARY KEY (`Name`),
    CONSTRAINT `FK_Types_Categories_DefaultCategoryName` FOREIGN KEY (`DefaultCategoryName`) REFERENCES `Categories` (`Name`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `LedgerEntries` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `IsIncome` tinyint(1) NOT NULL,
    `CreateTime` datetime(6) NOT NULL,
    `GivenTime` datetime(6) NOT NULL,
    `Amount` decimal(10,2) NOT NULL,
    `Description` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `TypeName` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `CategoryName` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_LedgerEntries` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LedgerEntries_Categories_CategoryName` FOREIGN KEY (`CategoryName`) REFERENCES `Categories` (`Name`) ON DELETE CASCADE,
    CONSTRAINT `FK_LedgerEntries_Types_TypeName` FOREIGN KEY (`TypeName`) REFERENCES `Types` (`Name`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_Categories_SuperCategoryName` ON `Categories` (`SuperCategoryName`);

CREATE INDEX `IX_LedgerEntries_CategoryName` ON `LedgerEntries` (`CategoryName`);

CREATE INDEX `IX_LedgerEntries_TypeName` ON `LedgerEntries` (`TypeName`);

CREATE INDEX `IX_Types_DefaultCategoryName` ON `Types` (`DefaultCategoryName`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230102124705_init', '7.0.1');

COMMIT;

