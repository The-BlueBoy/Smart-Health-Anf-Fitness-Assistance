-- ============================================================
-- Run this once in SQL Server Object Explorer:
-- Right-click your LocalDB instance -> New Query -> paste all of this -> Execute (F5)
-- ============================================================

IF DB_ID('WellnessDb') IS NULL
BEGIN
    CREATE DATABASE WellnessDb;
END
GO

USE WellnessDb;
GO

IF OBJECT_ID('dbo.UserProfiles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserProfiles
    (
        ProfileId           INT IDENTITY(1,1) PRIMARY KEY,
        FullName            NVARCHAR(50)   NOT NULL,
        Age                 INT            NOT NULL,
        Gender              NVARCHAR(10)   NOT NULL,
        HeightCm            FLOAT          NOT NULL,
        WeightKg            FLOAT          NOT NULL,
        ActivityLevel       NVARCHAR(20)   NOT NULL,
        Goal                NVARCHAR(30)   NOT NULL,
        Diet                NVARCHAR(20)   NOT NULL,
        SleepHours          FLOAT          NOT NULL,
        WorkoutDaysPerWeek  INT            NOT NULL,
        Notes               NVARCHAR(300)  NULL,
        CreatedOn           DATETIME       NOT NULL DEFAULT GETDATE()
    );
END
GO
