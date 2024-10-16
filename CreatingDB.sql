-- 1. Создание базы данных
CREATE DATABASE IF NOT EXISTS scatterplot_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE scatterplot_db;

-- 2. Создание таблицы AspNetRoles
CREATE TABLE IF NOT EXISTS AspNetRoles (
    Id VARCHAR(191) NOT NULL PRIMARY KEY,
    Name VARCHAR(256),
    NormalizedName VARCHAR(256),
    ConcurrencyStamp VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Создание таблицы AspNetUsers
CREATE TABLE IF NOT EXISTS AspNetUsers (
    Id VARCHAR(191) NOT NULL PRIMARY KEY,
    UserName VARCHAR(256),
    NormalizedUserName VARCHAR(256),
    Email VARCHAR(256),
    NormalizedEmail VARCHAR(256),
    EmailConfirmed BOOLEAN NOT NULL,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    ConcurrencyStamp TEXT,
    PhoneNumber VARCHAR(20),
    PhoneNumberConfirmed BOOLEAN NOT NULL,
    TwoFactorEnabled BOOLEAN NOT NULL,
    LockoutEnd DATETIME(6),
    LockoutEnabled BOOLEAN NOT NULL,
    AccessFailedCount INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Создание таблицы AspNetRoleClaims
CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    RoleId VARCHAR(191) NOT NULL,
    ClaimType VARCHAR(255),
    ClaimValue VARCHAR(255),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. Создание таблицы AspNetUserClaims
CREATE TABLE IF NOT EXISTS AspNetUserClaims (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(191) NOT NULL,
    ClaimType VARCHAR(255),
    ClaimValue VARCHAR(255),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6. Создание таблицы AspNetUserLogins
CREATE TABLE IF NOT EXISTS AspNetUserLogins (
    LoginProvider VARCHAR(128) NOT NULL,
    ProviderKey VARCHAR(128) NOT NULL,
    ProviderDisplayName VARCHAR(255),
    UserId VARCHAR(191) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7. Создание таблицы AspNetUserRoles
CREATE TABLE IF NOT EXISTS AspNetUserRoles (
    UserId VARCHAR(191) NOT NULL,
    RoleId VARCHAR(191) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 8. Создание таблицы AspNetUserTokens
CREATE TABLE IF NOT EXISTS AspNetUserTokens (
    UserId VARCHAR(191) NOT NULL,
    LoginProvider VARCHAR(128) NOT NULL,
    Name VARCHAR(128) NOT NULL,
    Value TEXT,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 9. Создание таблицы ChartDatas
CREATE TABLE IF NOT EXISTS ChartDatas (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(191) NOT NULL,
    XValues TEXT NOT NULL, -- Хранение значений X (например, JSON или CSV)
    YValues TEXT NOT NULL, -- Хранение значений Y
    ChartImagePath VARCHAR(255) NOT NULL, -- Путь к изображению диаграммы
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 10. Создание индексов

-- Индекс для нормализованного имени роли
CREATE INDEX IX_AspNetRoles_NormalizedName ON AspNetRoles(NormalizedName);

-- Индекс для нормализованного имени пользователя
CREATE INDEX IX_AspNetUsers_NormalizedUserName ON AspNetUsers(NormalizedUserName);

-- Индекс для нормализованного email пользователя
CREATE INDEX IX_AspNetUsers_NormalizedEmail ON AspNetUsers(NormalizedEmail);

-- Индекс для UserId в ChartDatas
CREATE INDEX IX_ChartDatas_UserId ON ChartDatas(UserId);
