-- ============================================================
-- SEEDER DANYCH TRANSAKCYJNYCH - MedicalCenterDB (PANCERNY)
-- ============================================================

USE MedicalCenterDB;
GO

PRINT '=== Start seedowania ===';
GO

DISABLE TRIGGER trg_Appointment_StatusHistory ON dbo.Appointments;
GO

-- ------------------------------------------------------------
-- TABELE TYMCZASOWE z numerami
-- ------------------------------------------------------------
IF OBJECT_ID('tempdb..#Patients') IS NOT NULL DROP TABLE #Patients;
IF OBJECT_ID('tempdb..#Doctors') IS NOT NULL DROP TABLE #Doctors;
IF OBJECT_ID('tempdb..#Medicines') IS NOT NULL DROP TABLE #Medicines;
IF OBJECT_ID('tempdb..#Couriers') IS NOT NULL DROP TABLE #Couriers;

SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) - 1 AS Idx INTO #Patients FROM dbo.Patients;
SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) - 1 AS Idx INTO #Doctors FROM dbo.Doctors;
SELECT Id, Price, ROW_NUMBER() OVER (ORDER BY Id) - 1 AS Idx INTO #Medicines FROM dbo.Medicines;
SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) - 1 AS Idx INTO #Couriers FROM dbo.Couriers;

CREATE INDEX IX_TempPatients ON #Patients(Idx);
CREATE INDEX IX_TempDoctors ON #Doctors(Idx);
CREATE INDEX IX_TempMedicines ON #Medicines(Idx);
CREATE INDEX IX_TempCouriers ON #Couriers(Idx);
GO

-- ============================================================
-- SEKCJA 1: APPOINTMENTS (50 000)
-- ============================================================
PRINT '--- 1. Appointments ---';
DECLARE @PatCount INT = (SELECT COUNT(*) FROM #Patients);
DECLARE @DocCount INT = (SELECT COUNT(*) FROM #Doctors);
DECLARE @Existing INT = (SELECT COUNT(*) FROM dbo.Appointments);

IF @Existing < 50000 AND @PatCount > 0 AND @DocCount > 0
BEGIN
    DECLARE @ToInsert INT = 50000 - @Existing;
    
    ;WITH Numbers AS (
        SELECT TOP (@ToInsert) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
        FROM sys.all_columns a CROSS JOIN sys.all_columns b
    )
    INSERT INTO dbo.Appointments (Id, PatientId, DoctorId, StatusId, AppointmentDate, DurationMinutes, Description, Notes)
    SELECT
        NEWID(),
        p.Id,
        d.Id,
        (n.n % 3) + 1,
        -- Haszowanie bez przepełnienia i wybuchu optymalizatora
        DATEADD(MINUTE, (n.n * 13) % 1051200, '2024-01-01'),
        30,
        N'Wizyta wygenerowana',
        NULL
    FROM Numbers n
    -- Deterministyczne łączenie matematyczne (chroni przed Table Spool)
    JOIN #Patients p ON p.Idx = (n.n * 73) % @PatCount
    JOIN #Doctors d ON d.Idx = (n.n * 37) % @DocCount;
END;
GO

-- ============================================================
-- SEKCJA 2: MEDICAL RECORDS 
-- ============================================================
PRINT '--- 2. MedicalRecords ---';
;WITH UniquePairs AS (
    SELECT DISTINCT PatientId, DoctorId FROM dbo.Appointments
)
INSERT INTO dbo.MedicalRecords (Id, PatientId, DoctorId)
SELECT NEWID(), up.PatientId, up.DoctorId
FROM UniquePairs up
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.MedicalRecords mr
    WHERE mr.PatientId = up.PatientId AND mr.DoctorId = up.DoctorId
);
GO

-- ============================================================
-- SEKCJA 3: DIAGNOSES
-- ============================================================
PRINT '--- 3. Diagnoses ---';
INSERT INTO dbo.Diagnoses (Id, MedicalRecordId, Description, DiagnosedAt)
SELECT
    NEWID(),
    mr.Id,
    CHOOSE((ABS(CAST(CHECKSUM(mr.Id) AS BIGINT)) % 8) + 1,
        N'Nadciśnienie tętnicze', N'Cukrzyca typu 2', N'Astma oskrzelowa', N'Migrena',
        N'Zapalenie zatok', N'Niedoczynność tarczycy', N'Choroba refluksowa przełyku', N'Atopowe zapalenie skóry'),
    DATEADD(DAY, -(ABS(CAST(CHECKSUM(mr.Id) AS BIGINT)) % 365), SYSDATETIME())
FROM dbo.MedicalRecords mr
WHERE ABS(CAST(CHECKSUM(mr.Id) AS BIGINT)) % 10 < 6
  AND NOT EXISTS (SELECT 1 FROM dbo.Diagnoses d WHERE d.MedicalRecordId = mr.Id);
GO

-- ============================================================
-- SEKCJA 4: TREATMENTS
-- ============================================================
PRINT '--- 4. Treatments ---';
INSERT INTO dbo.Treatments (Id, DiagnosisId, Description)
SELECT
    NEWID(),
    d.Id,
    CHOOSE((ABS(CAST(CHECKSUM(d.Id) AS BIGINT)) % 5) + 1,
        N'Farmakoterapia, kontrola za 30 dni', N'Dieta, aktywność fizyczna, kontrola za 60 dni',
        N'Skierowanie do specjalisty', N'Obserwacja, kontrola za 14 dni', N'Modyfikacja stylu życia, kontrola za 90 dni')
FROM dbo.Diagnoses d
WHERE NOT EXISTS (SELECT 1 FROM dbo.Treatments t WHERE t.DiagnosisId = d.Id);
GO

-- ============================================================
-- SEKCJA 5: PRESCRIPTIONS
-- ============================================================
PRINT '--- 5. Prescriptions ---';
INSERT INTO dbo.Prescriptions (Id, MedicalRecordId, DoctorId, IssuedAt)
SELECT
    NEWID(),
    mr.Id,
    mr.DoctorId,
    DATEADD(DAY, -(ABS(CAST(CHECKSUM(mr.Id) AS BIGINT)) % 365), SYSDATETIME())
FROM dbo.MedicalRecords mr
WHERE ABS(CAST(CHECKSUM(mr.Id) AS BIGINT)) % 10 < 5
  AND NOT EXISTS (SELECT 1 FROM dbo.Prescriptions p WHERE p.MedicalRecordId = mr.Id);
GO

-- ============================================================
-- SEKCJA 6: PRESCRIPTION ITEMS
-- ============================================================
PRINT '--- 6. PrescriptionItems ---';
DECLARE @MedCount INT = (SELECT COUNT(*) FROM #Medicines);

IF @MedCount > 0
BEGIN
    INSERT INTO dbo.PrescriptionItems (Id, PrescriptionId, MedicineId, Quantity, Notes)
    SELECT
        NEWID(),
        p.Id,
        m.Id,
        (ABS(CAST(CHECKSUM(p.Id) AS BIGINT)) % 3) + 1,
        NULL
    FROM dbo.Prescriptions p
    JOIN #Medicines m ON m.Idx = ABS(CAST(CHECKSUM(p.Id) AS BIGINT)) % @MedCount
    WHERE NOT EXISTS (SELECT 1 FROM dbo.PrescriptionItems pi WHERE pi.PrescriptionId = p.Id);
END;
GO

-- ============================================================
-- SEKCJA 7: ORDERS (15 000)
-- ============================================================
PRINT '--- 7. Orders ---';
DECLARE @PatCount2 INT = (SELECT COUNT(*) FROM #Patients);
DECLARE @ExistingOrders INT = (SELECT COUNT(*) FROM dbo.Orders);

IF @ExistingOrders < 15000 AND @PatCount2 > 0
BEGIN
    DECLARE @ToInsertOrders INT = 15000 - @ExistingOrders;
    
    ;WITH Numbers AS (
        SELECT TOP (@ToInsertOrders) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
        FROM sys.all_columns a CROSS JOIN sys.all_columns b
    )
    INSERT INTO dbo.Orders (Id, PatientId, TotalPrice, StatusId, CreatedAt, StripeSessionId)
    SELECT
        NEWID(),
        p.Id,
        CAST(((n.n * 17) % 500) + 10 AS DECIMAL(10,2)),
        (n.n % 3) + 1,
        DATEADD(DAY, -((n.n * 11) % 730), SYSDATETIME()),
        NULL
    FROM Numbers n
    JOIN #Patients p ON p.Idx = (n.n * 43) % @PatCount2;
END;
GO

-- ============================================================
-- SEKCJA 8: ORDER ITEMS
-- ============================================================
PRINT '--- 8. OrderItems ---';
DECLARE @MedCount2 INT = (SELECT COUNT(*) FROM #Medicines);

IF @MedCount2 > 0
BEGIN
    ;WITH OrdersWithoutItems AS (
        SELECT o.Id FROM dbo.Orders o WHERE NOT EXISTS (SELECT 1 FROM dbo.OrderItems oi WHERE oi.OrderId = o.Id)
    ),
    OrderMultiplier AS (
        SELECT o.Id AS OrderId, n.n
        FROM OrdersWithoutItems o
        CROSS JOIN (VALUES (1),(2),(3)) AS n(n)
        -- Generujemy od 1 do 2 leków per zamówienie
        WHERE (ABS(CAST(CHECKSUM(o.Id) AS BIGINT)) + n.n) % 3 < 2
    )
    INSERT INTO dbo.OrderItems (Id, OrderId, MedicineId, Quantity, UnitPrice)
    SELECT
        NEWID(),
        om.OrderId,
        m.Id,
        (ABS(CAST(CHECKSUM(om.OrderId) AS BIGINT)) % 5) + 1,
        m.Price
    FROM OrderMultiplier om
    -- To gwarantuje unikalność par OrderId-MedicineId bez błędów!
    JOIN #Medicines m ON m.Idx = (ABS(CAST(CHECKSUM(om.OrderId) AS BIGINT)) + om.n) % @MedCount2;
END;
GO

-- ============================================================
-- SEKCJA 9: DELIVERIES
-- ============================================================
PRINT '--- 9. Deliveries ---';
DECLARE @CourierCount INT = (SELECT COUNT(*) FROM #Couriers);

IF @CourierCount > 0
BEGIN
    INSERT INTO dbo.Deliveries (Id, OrderId, CourierId, StatusId, DeliveredAt)
    SELECT
        NEWID(),
        o.Id,
        c.Id,
        (ABS(CAST(CHECKSUM(o.Id) AS BIGINT)) % 3) + 1,
        CASE WHEN (ABS(CAST(CHECKSUM(o.Id) AS BIGINT)) % 2) = 0 
             THEN DATEADD(DAY, (ABS(CAST(CHECKSUM(o.Id) AS BIGINT)) % 14), o.CreatedAt) ELSE NULL END
    FROM dbo.Orders o
    JOIN #Couriers c ON c.Idx = ABS(CAST(CHECKSUM(o.Id) AS BIGINT)) % @CourierCount
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Deliveries d WHERE d.OrderId = o.Id);
END;
GO

-- ============================================================
-- SEKCJA 10-13 
-- ============================================================
PRINT '--- 10. Invoices ---';
INSERT INTO dbo.Invoices (Id, PatientId, OrderId, Amount, TaxAmount, TotalAmount, IssuedAt, StripePaymentId)
SELECT NEWID(), o.PatientId, o.Id, o.TotalPrice / 1.23, o.TotalPrice - (o.TotalPrice / 1.23), o.TotalPrice, DATEADD(DAY, 1, o.CreatedAt), N'pi_test_' + LEFT(CONVERT(NVARCHAR(40), NEWID()), 20)
FROM dbo.Orders o WHERE o.StatusId = 2 AND NOT EXISTS (SELECT 1 FROM dbo.Invoices i WHERE i.OrderId = o.Id);

PRINT '--- 11. Reviews ---';
;WITH UniquePairs AS (
    SELECT DISTINCT a.PatientId, a.DoctorId FROM dbo.Appointments a WHERE ABS(CAST(CHECKSUM(a.Id) AS BIGINT)) % 10 < 3
)
INSERT INTO dbo.Reviews (Id, PatientId, DoctorId, Rating, Comment, CreatedAt)
SELECT NEWID(), up.PatientId, up.DoctorId, ABS(CAST(CHECKSUM(up.PatientId) AS BIGINT)) % 5 + 1,
    CHOOSE(ABS(CAST(CHECKSUM(up.DoctorId) AS BIGINT)) % 4 + 1, N'Bardzo dobry lekarz, polecam', N'Profesjonalna obsługa', N'Krótko i konkretnie', NULL),
    DATEADD(DAY, -(ABS(CAST(CHECKSUM(up.PatientId) AS BIGINT)) % 365), SYSDATETIME())
FROM UniquePairs up WHERE NOT EXISTS (SELECT 1 FROM dbo.Reviews r WHERE r.PatientId = up.PatientId AND r.DoctorId = up.DoctorId);

PRINT '--- 12. Referral ---';
INSERT INTO dbo.Referral (Id, DoctorId, PatientId, TargetSpecialization, Description, IssuedDate, ExpiryDate)
SELECT NEWID(), a.DoctorId, a.PatientId, CHOOSE((ABS(CAST(CHECKSUM(a.Id) AS BIGINT)) % 5) + 1, N'Kardiolog', N'Neurolog', N'Ortopeda', N'Endokrynolog', N'Onkolog'),
    N'Skierowanie wystawione na podstawie wizyty kontrolnej', a.AppointmentDate, DATEADD(MONTH, 3, a.AppointmentDate)
FROM dbo.Appointments a WHERE ABS(CAST(CHECKSUM(a.Id) AS BIGINT)) % 10 = 0 AND NOT EXISTS (SELECT 1 FROM dbo.Referral r WHERE r.PatientId = a.PatientId AND r.DoctorId = a.DoctorId AND r.IssuedDate = a.AppointmentDate);

PRINT '--- 13. MedicalLeaves ---';
INSERT INTO dbo.MedicalLeaves (Id, PatientId, DoctorId, DateFrom, DateTo, Reason, IssuedAt)
SELECT NEWID(), a.PatientId, a.DoctorId, a.AppointmentDate, DATEADD(DAY, (ABS(CAST(CHECKSUM(a.Id) AS BIGINT)) % 14) + 1, a.AppointmentDate),
    COALESCE(CHOOSE((ABS(CAST(CHECKSUM(a.Id) AS BIGINT)) % 3) + 1, N'Infekcja wirusowa', N'Zabieg operacyjny - rekonwalescencja', N'Niezdolność do pracy'), N'Inne'), a.AppointmentDate
FROM dbo.Appointments a WHERE ABS(CAST(CHECKSUM(a.Id) AS BIGINT)) % 20 = 0 AND NOT EXISTS (SELECT 1 FROM dbo.MedicalLeaves ml WHERE ml.PatientId = a.PatientId AND ml.DoctorId = a.DoctorId AND ml.DateFrom = a.AppointmentDate);
GO

-- ------------------------------------------------------------
-- SPRZĄTANIE
-- ------------------------------------------------------------
DROP TABLE IF EXISTS #Patients;
DROP TABLE IF EXISTS #Doctors;
DROP TABLE IF EXISTS #Medicines;
DROP TABLE IF EXISTS #Couriers;

ENABLE TRIGGER trg_Appointment_StatusHistory ON dbo.Appointments;
GO
PRINT '=== Koniec ===';