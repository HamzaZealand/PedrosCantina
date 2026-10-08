-- SQL-forespørgsler til Pedros Cantina.
-- Disse queries bruges i C#-programmet via ADO.NET med parametre.

USE PedrosCantina;
GO

-- 1) Månedsplan
-- Viser dato, start, slut, medarbejder og om personen er leder.
SELECT s.ShiftDate,
       s.StartTime,
       s.EndTime,
       e.Name,
       e.IsManager
FROM Shift s
INNER JOIN ShiftAssignment sa ON s.ShiftId = sa.ShiftId
INNER JOIN Employee e ON sa.EmployeeId = e.EmployeeId
WHERE YEAR(s.ShiftDate) = @Year
  AND MONTH(s.ShiftDate) = @Month
ORDER BY s.ShiftDate, s.StartTime, e.Name;

-- Eksempel:
-- WHERE YEAR(s.ShiftDate) = 2026 AND MONTH(s.ShiftDate) = 10


-- 2) Månedlig belastning
-- Viser medarbejder, antal vagter og arbejdstimer for en måned.
-- En vagt på 09:00-14:00 eller 14:00-19:00 tæller som 5 timer.
SELECT e.Name,
       COUNT(*) AS ShiftCount,
       SUM(DATEDIFF(MINUTE, s.StartTime, s.EndTime) / 60.0) AS Hours
FROM Employee e
INNER JOIN ShiftAssignment sa ON e.EmployeeId = sa.EmployeeId
INNER JOIN Shift s ON sa.ShiftId = s.ShiftId
WHERE YEAR(s.ShiftDate) = @Year
  AND MONTH(s.ShiftDate) = @Month
GROUP BY e.Name
ORDER BY e.Name;


-- 3) Kontaktinformation
-- Kan bruges, hvis en medarbejder er syg, og en anden skal kontaktes.
SELECT EmployeeId,
       Name,
       Phone,
       Email
FROM Employee
WHERE EmployeeId = @EmployeeId;

-- Søgning på navn:
SELECT EmployeeId,
       Name,
       Phone,
       Email
FROM Employee
WHERE Name LIKE @Name
ORDER BY Name;


-- 4) Årlig belastning
-- Viser belastning pr. medarbejder pr. måned i et valgt år.
SELECT e.Name,
       MONTH(s.ShiftDate) AS MonthNumber,
       COUNT(*) AS ShiftCount,
       SUM(DATEDIFF(MINUTE, s.StartTime, s.EndTime) / 60.0) AS Hours
FROM Employee e
INNER JOIN ShiftAssignment sa ON e.EmployeeId = sa.EmployeeId
INNER JOIN Shift s ON sa.ShiftId = s.ShiftId
WHERE YEAR(s.ShiftDate) = @Year
GROUP BY e.Name, MONTH(s.ShiftDate)
ORDER BY e.Name, MONTH(s.ShiftDate);


-- 5) Ny månedsplan: opret vagt
INSERT INTO Shift (ShiftDate, StartTime, EndTime)
VALUES (@ShiftDate, @StartTime, @EndTime);


-- 6) Justering: tildel medarbejder til vagt
INSERT INTO ShiftAssignment (ShiftId, EmployeeId)
VALUES (@ShiftId, @EmployeeId);


-- 7) Justering: fjern medarbejder fra vagt
DELETE FROM ShiftAssignment
WHERE ShiftId = @ShiftId
  AND EmployeeId = @EmployeeId;


-- 8) Employee CRUD (bruges af EmployeeRepository)

-- Create
INSERT INTO Employee (Name, Phone, Email, IsManager)
VALUES (@Name, @Phone, @Email, @IsManager);

-- Read all
SELECT EmployeeId, Name, Phone, Email, IsManager
FROM Employee
ORDER BY Name;

-- Read one
SELECT EmployeeId, Name, Phone, Email, IsManager
FROM Employee
WHERE EmployeeId = @EmployeeId;

-- Update
UPDATE Employee
SET Name = @Name,
    Phone = @Phone,
    Email = @Email,
    IsManager = @IsManager
WHERE EmployeeId = @EmployeeId;

-- Delete
DELETE FROM Employee
WHERE EmployeeId = @EmployeeId;
