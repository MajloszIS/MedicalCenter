CREATE OR ALTER PROCEDURE dbo.usp_CancelUnpaidOrders
    @CancelledCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OrderId UNIQUEIDENTIFIER;
    DECLARE @PendingStatusId INT = 1;   -- NOWE
    DECLARE @CancelledStatusId INT = 5; -- ANULOWANE
    
    SET @CancelledCount = 0;

    DECLARE cur_UnpaidOrders CURSOR LOCAL FAST_FORWARD FOR
        SELECT Id
        FROM dbo.[Orders]
        WHERE StatusId = @PendingStatusId
          AND CreatedAt < DATEADD(hour, -48, GETDATE());

    OPEN cur_UnpaidOrders;
    FETCH NEXT FROM cur_UnpaidOrders INTO @OrderId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            UPDATE dbo.[Orders]
               SET StatusId = @CancelledStatusId
             WHERE Id = @OrderId;

            UPDATE m
               SET m.StockQuantity = m.StockQuantity + oi.Quantity
              FROM dbo.Medicines m
             INNER JOIN dbo.OrderItems oi ON m.Id = oi.MedicineId
             WHERE oi.OrderId = @OrderId;

            COMMIT TRANSACTION;
            
            SET @CancelledCount = @CancelledCount + 1;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
        END CATCH

        FETCH NEXT FROM cur_UnpaidOrders INTO @OrderId;
    END

    CLOSE cur_UnpaidOrders;
    DEALLOCATE cur_UnpaidOrders;
END;
GO