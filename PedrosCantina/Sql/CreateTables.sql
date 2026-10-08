-- Opret tabellerne i PedrosCantina.
-- Kør dette script efter CreateDatabase.sql.
-- Vælg databasen PedrosCantina, før du kører scriptet.

USE PedrosCantina;
GO

IF OBJECT_ID(N'dbo.ShiftAssignment', N'U') IS NOT NULL
    DROP TABLE dbo.ShiftAssignment;
GO

IF OBJECT_ID(N'dbo.Shift', N'U') IS NOT NULL
    DROP TABLE dbo.Shift;
GO

IF OBJECT_ID(N'dbo.Employee', N'U') IS NOT NULL
    DROP TABLE dbo.Employee;
GO

CREATE TABLE dbo.Employee
(
    EmployeeId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    IsManager BIT NOT NULL CONSTRAINT DF_Employee_IsManager DEFAULT (0),
    CONSTRAINT PK_Employee PRIMARY KEY (EmployeeId)
);
GO

CREATE TABLE dbo.Shift
(
    ShiftId INT IDENTITY(1,1) NOT NULL,
    ShiftDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    CONSTRAINT PK_Shift PRIMARY KEY (ShiftId),
    CONSTRAINT CK_Shift_StartBeforeEnd CHECK (StartTime < EndTime),
    CONSTRAINT CK_Shift_StandardTimes CHECK
    (
        (StartTime = '09:00' AND EndTime = '14:00')
        OR
        (StartTime = '14:00' AND EndTime = '19:00')
    ),
    CONSTRAINT UQ_Shift_DateAndStart UNIQUE (ShiftDate, StartTime)
);
GO

CREATE TABLE dbo.ShiftAssignment
(
    ShiftId INT NOT NULL,
    EmployeeId INT NOT NULL,
    CONSTRAINT PK_ShiftAssignment PRIMARY KEY (ShiftId, EmployeeId),
    CONSTRAINT FK_ShiftAssignment_Shift
        FOREIGN KEY (ShiftId) REFERENCES dbo.Shift (ShiftId),
    CONSTRAINT FK_ShiftAssignment_Employee
        FOREIGN KEY (EmployeeId) REFERENCES dbo.Employee (EmployeeId)
);
GO
