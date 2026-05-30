--dodanie schemy

USE MedicalCenterDB;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'rpt')
    EXEC('CREATE SCHEMA rpt');
GO

-- dodanie tabeli do raportu lekarzy o danej spec
CREATE TABLE rpt.SpecializationMonthlyReports (
    Id              INT IDENTITY PRIMARY KEY,
    ReportYear      INT NOT NULL,
    ReportMonth     INT NOT NULL CHECK (ReportMonth BETWEEN 1 AND 12),
    SpecializationId UNIQUEIDENTIFIER NOT NULL,
    SpecializationName NVARCHAR(200) NOT NULL,
    ActiveDoctorsCount  INT NOT NULL DEFAULT 0,
    UniquePatientsCount INT NOT NULL DEFAULT 0,
    ScheduledCount      INT NOT NULL DEFAULT 0,
    CompletedCount      INT NOT NULL DEFAULT 0,
    CancelledCount      INT NOT NULL DEFAULT 0,
    PrescriptionsIssued INT NOT NULL DEFAULT 0,
    GeneratedAt         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_SpecReport UNIQUE (ReportYear, ReportMonth, SpecializationId)
);
GO

--dodanie procedury
CREATE OR ALTER PROCEDURE rpt.usp_MonthlySpecializationReport
    @Year  INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Month NOT BETWEEN 1 AND 12
        THROW 60001, 'Miesiąc musi być w zakresie 1-12.', 1;
    IF @Year < 2000 OR @Year > YEAR(SYSDATETIME()) + 1
        THROW 60002, 'Niepoprawny rok.', 1;

    DECLARE @StartDate DATETIME2 = DATETIMEFROMPARTS(@Year, @Month, 1, 0, 0, 0, 0);
    DECLARE @EndDate   DATETIME2 = DATEADD(MONTH, 1, @StartDate);

    DELETE FROM rpt.SpecializationMonthlyReports
    WHERE ReportYear = @Year AND ReportMonth = @Month;

    DECLARE @SpecId   UNIQUEIDENTIFIER,
            @SpecName NVARCHAR(200);

    DECLARE @ActiveDoctors      INT,
            @UniquePatients     INT,
            @Scheduled          INT,
            @Completed          INT,
            @Cancelled          INT,
            @Prescriptions      INT;

    DECLARE spec_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT Id, Name
        FROM dbo.Specializations
        ORDER BY Name;

    OPEN spec_cursor;
    FETCH NEXT FROM spec_cursor INTO @SpecId, @SpecName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @ActiveDoctors = COUNT(DISTINCT d.Id)
        FROM dbo.Doctors d
        INNER JOIN dbo.Appointments a ON a.DoctorId = d.Id
        WHERE d.SpecializationId = @SpecId
          AND a.AppointmentDate >= @StartDate
          AND a.AppointmentDate <  @EndDate;

        SELECT @UniquePatients = COUNT(DISTINCT a.PatientId)
        FROM dbo.Appointments a
        INNER JOIN dbo.Doctors d ON a.DoctorId = d.Id
        WHERE d.SpecializationId = @SpecId
          AND a.AppointmentDate >= @StartDate
          AND a.AppointmentDate <  @EndDate;

        -- Statusy: 'Zaplanowana', 'Zakończona', 'Anulowana'
        SELECT
            @Scheduled = SUM(CASE WHEN s.Name = N'Zaplanowana' THEN 1 ELSE 0 END),
            @Completed = SUM(CASE WHEN s.Name = N'Zakończona'  THEN 1 ELSE 0 END),
            @Cancelled = SUM(CASE WHEN s.Name = N'Anulowana'   THEN 1 ELSE 0 END)
        FROM dbo.Appointments a
        INNER JOIN dbo.Doctors d ON a.DoctorId = d.Id
        INNER JOIN dbo.AppointmentStatuses s ON s.Id = a.StatusId
        WHERE d.SpecializationId = @SpecId
          AND a.AppointmentDate >= @StartDate
          AND a.AppointmentDate <  @EndDate;

        -- Recepty: kolumna nazywa się IssuedAt
        SELECT @Prescriptions = COUNT(*)
        FROM dbo.Prescriptions p
        INNER JOIN dbo.Doctors d ON p.DoctorId = d.Id
        WHERE d.SpecializationId = @SpecId
          AND p.IssuedAt >= @StartDate
          AND p.IssuedAt <  @EndDate;

        INSERT INTO rpt.SpecializationMonthlyReports
            (ReportYear, ReportMonth, SpecializationId, SpecializationName,
             ActiveDoctorsCount, UniquePatientsCount,
             ScheduledCount, CompletedCount, CancelledCount,
             PrescriptionsIssued)
        VALUES
            (@Year, @Month, @SpecId, @SpecName,
             ISNULL(@ActiveDoctors, 0), ISNULL(@UniquePatients, 0),
             ISNULL(@Scheduled, 0), ISNULL(@Completed, 0), ISNULL(@Cancelled, 0),
             ISNULL(@Prescriptions, 0));

        FETCH NEXT FROM spec_cursor INTO @SpecId, @SpecName;
    END

    CLOSE spec_cursor;
    DEALLOCATE spec_cursor;

    SELECT *
    FROM rpt.SpecializationMonthlyReports
    WHERE ReportYear = @Year AND ReportMonth = @Month
    ORDER BY SpecializationName;
END;
GO


-- nadanie uprawnień dla analityka
GRANT EXECUTE ON rpt.usp_MonthlySpecializationReport TO db_analyst;
GRANT SELECT  ON rpt.SpecializationMonthlyReports     TO db_analyst;

-- wykonanie procedury
EXEC rpt.usp_MonthlySpecializationReport @Year = 2026, @Month = 5;