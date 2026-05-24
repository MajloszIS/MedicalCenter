CREATE OR ALTER PROCEDURE dbo.usp_AppointmentReserve
    @PatientId         UNIQUEIDENTIFIER,
    @DoctorId          UNIQUEIDENTIFIER,
    @AppointmentDate   DATETIME2(7),
    @Description       NVARCHAR(MAX) = N'',
    @Notes             NVARCHAR(MAX) = N'',
    @AppointmentId     UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @AppointmentDate <= SYSDATETIME()
            THROW 50001, 'Termin wizyty musi być w przyszłości.', 1;

        IF NOT EXISTS (SELECT 1 FROM dbo.Patients WHERE Id = @PatientId)
            THROW 50002, 'Pacjent o podanym Id nie istnieje.', 1;

        IF NOT EXISTS (SELECT 1 FROM dbo.Doctors WHERE Id = @DoctorId)
            THROW 50003, 'Lekarz o podanym Id nie istnieje.', 1;

        IF EXISTS (
            SELECT 1
            FROM dbo.Appointments a
            INNER JOIN dbo.AppointmentStatuses s ON s.Id = a.StatusId
            WHERE a.DoctorId = @DoctorId
              AND a.AppointmentDate = @AppointmentDate
              AND s.Name <> N'Anulowana'
        )
            THROW 50004, 'Lekarz ma już wizytę w tym terminie.', 1;

        IF EXISTS (
            SELECT 1
            FROM dbo.Appointments a
            INNER JOIN dbo.AppointmentStatuses s ON s.Id = a.StatusId
            WHERE a.PatientId = @PatientId
              AND a.AppointmentDate = @AppointmentDate
              AND s.Name <> N'Anulowana'
        )
            THROW 50005, 'Pacjent ma już wizytę w tym terminie.', 1;

        -- TYLKO ta zmienna zmienia typ na INT
        DECLARE @StatusId INT;
        SELECT @StatusId = Id
          FROM dbo.AppointmentStatuses
         WHERE Name = N'Zaplanowana';

        IF @StatusId IS NULL
            THROW 50006, 'Brak statusu "Zaplanowana" w słowniku statusów.', 1;

        SET @AppointmentId = NEWID();

        INSERT INTO dbo.Appointments (Id, PatientId, DoctorId, StatusId, AppointmentDate, Description, Notes)
        VALUES (@AppointmentId, @PatientId, @DoctorId, @StatusId, @AppointmentDate, @Description, @Notes);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;