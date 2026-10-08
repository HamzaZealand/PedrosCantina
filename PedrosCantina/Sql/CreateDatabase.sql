-- Opret databasen PedrosCantina.
-- Kør dette script først i SQL Server Management Studio (mod master).

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PedrosCantina')
BEGIN
    CREATE DATABASE PedrosCantina;
END
GO
