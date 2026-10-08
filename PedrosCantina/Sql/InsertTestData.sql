-- Testdata til Pedros Cantina.
-- Kør dette script efter CreateTables.sql.
-- Vagterne er spredt over flere måneder i 2026, så månedsplan,
-- månedlig belastning og årlig belastning kan demonstreres.
-- Færdige vagter har 2-3 personer og mindst én leder.

USE PedrosCantina;
GO

DELETE FROM dbo.ShiftAssignment;
DELETE FROM dbo.Shift;
DELETE FROM dbo.Employee;
GO

SET IDENTITY_INSERT dbo.Employee ON;

INSERT INTO dbo.Employee (EmployeeId, Name, Phone, Email, IsManager)
VALUES
    (1, N'Pedro Gonzalez',  N'20112233', N'pedro@pedroscantina.dk',  1),
    (2, N'Jacoby Nielsen',  N'20223344', N'jacoby@pedroscantina.dk', 1),
    (3, N'Maria Hansen',    N'30334455', N'maria@pedroscantina.dk',  0),
    (4, N'Anders Larsen',   N'40445566', N'anders@pedroscantina.dk', 0),
    (5, N'Sofia Berg',      N'50556677', N'sofia@pedroscantina.dk',  0),
    (6, N'Emil Kristensen', N'60667788', N'emil@pedroscantina.dk',   0);

SET IDENTITY_INSERT dbo.Employee OFF;
DBCC CHECKIDENT ('dbo.Employee', RESEED, 6);
GO

SET IDENTITY_INSERT dbo.Shift ON;

INSERT INTO dbo.Shift (ShiftId, ShiftDate, StartTime, EndTime)
VALUES
    -- Januar 2026
    (1,  '2026-01-01', '09:00', '14:00'),
    (2,  '2026-01-01', '14:00', '19:00'),
    (3,  '2026-01-02', '09:00', '14:00'),
    (4,  '2026-01-02', '14:00', '19:00'),
    (5,  '2026-01-03', '09:00', '14:00'),
    (6,  '2026-01-03', '14:00', '19:00'),

    -- Marts 2026
    (7,  '2026-03-02', '09:00', '14:00'),
    (8,  '2026-03-02', '14:00', '19:00'),
    (9,  '2026-03-03', '09:00', '14:00'),
    (10, '2026-03-03', '14:00', '19:00'),

    -- Juni 2026
    (11, '2026-06-01', '09:00', '14:00'),
    (12, '2026-06-01', '14:00', '19:00'),
    (13, '2026-06-02', '09:00', '14:00'),
    (14, '2026-06-02', '14:00', '19:00'),

    -- Oktober 2026 (en uge til månedsplan)
    (15, '2026-10-05', '09:00', '14:00'),
    (16, '2026-10-05', '14:00', '19:00'),
    (17, '2026-10-06', '09:00', '14:00'),
    (18, '2026-10-06', '14:00', '19:00'),
    (19, '2026-10-07', '09:00', '14:00'),
    (20, '2026-10-07', '14:00', '19:00'),
    (21, '2026-10-08', '09:00', '14:00'),
    (22, '2026-10-08', '14:00', '19:00'),
    (23, '2026-10-09', '09:00', '14:00'),
    (24, '2026-10-09', '14:00', '19:00'),
    (25, '2026-10-10', '09:00', '14:00'),
    (26, '2026-10-10', '14:00', '19:00'),
    (27, '2026-10-11', '09:00', '14:00'),
    (28, '2026-10-11', '14:00', '19:00'),

    -- December 2026
    (29, '2026-12-23', '09:00', '14:00'),
    (30, '2026-12-23', '14:00', '19:00'),
    (31, '2026-12-24', '09:00', '14:00'),
    (32, '2026-12-24', '14:00', '19:00');

SET IDENTITY_INSERT dbo.Shift OFF;
DBCC CHECKIDENT ('dbo.Shift', RESEED, 32);
GO

-- EmployeeId: 1 Pedro, 2 Jacoby, 3 Maria, 4 Anders, 5 Sofia, 6 Emil
INSERT INTO dbo.ShiftAssignment (ShiftId, EmployeeId)
VALUES
    -- 01-01-2026 (hverdag)
    (1, 1), (1, 3),
    (2, 2), (2, 4),

    -- 02-01-2026 (hverdag)
    (3, 2), (3, 5),
    (4, 1), (4, 6),

    -- 03-01-2026 (lørdag)
    (5, 1), (5, 3), (5, 5),
    (6, 2), (6, 4), (6, 6),

    -- 02-03-2026 (hverdag)
    (7, 1), (7, 4),
    (8, 2), (8, 3),

    -- 03-03-2026 (hverdag)
    (9,  2), (9,  6),
    (10, 1), (10, 5),

    -- 01-06-2026 (hverdag)
    (11, 1), (11, 3),
    (12, 2), (12, 4),

    -- 02-06-2026 (hverdag)
    (13, 2), (13, 5),
    (14, 1), (14, 6),

    -- 05-10-2026 til 09-10-2026 (hverdage)
    (15, 1), (15, 3),
    (16, 2), (16, 4),
    (17, 2), (17, 5),
    (18, 1), (18, 6),
    (19, 1), (19, 4),
    (20, 2), (20, 3),
    (21, 2), (21, 6),
    (22, 1), (22, 5),
    (23, 1), (23, 3),
    (24, 2), (24, 4),

    -- 10-10-2026 og 11-10-2026 (weekend)
    (25, 1), (25, 3), (25, 5),
    (26, 2), (26, 4), (26, 6),
    (27, 2), (27, 5), (27, 6),
    (28, 1), (28, 3), (28, 4),

    -- 23-12-2026 og 24-12-2026 (hverdage)
    (29, 1), (29, 3),
    (30, 2), (30, 4),
    (31, 2), (31, 5),
    (32, 1), (32, 6);
GO
