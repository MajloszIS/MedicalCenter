USE MedicalCenterDB
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE rpt.usp_GetLowStockMedicines
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        m.Name AS MedicineName,
        m.StockQuantity AS CurrentStock,
        SUM(oi.Quantity) AS SoldInLast30Days,
        
        CAST(m.StockQuantity * 30.0 / NULLIF(SUM(oi.Quantity), 0) AS INT) AS EstimatedDaysToEmpty
        
    FROM dbo.Medicines m
    INNER JOIN dbo.OrderItems oi ON m.Id = oi.MedicineId
    INNER JOIN dbo.Orders o ON oi.OrderId = o.Id
    WHERE o.CreatedAt >= DATEADD(day, -30, GETDATE())
      AND o.StatusId != 5
    GROUP BY m.Name, m.StockQuantity
    HAVING
        CAST(m.StockQuantity * 30.0 / NULLIF(SUM(oi.Quantity), 0) AS INT) <= 7
        OR m.StockQuantity <= 0
    ORDER BY EstimatedDaysToEmpty ASC;
END;
