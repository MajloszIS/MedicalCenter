USE MedicalCenterDB; 
GO

CREATE OR ALTER FUNCTION rpt.fn_GetMedicineSalesStats
(
    @StartDate DATETIME2(7),
    @EndDate DATETIME2(7)
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        m.Name AS MedicineName,
        ISNULL(SUM(oi.Quantity), 0) AS TotalSoldPackages,
        ISNULL(SUM(oi.Quantity * oi.UnitPrice), 0) AS TotalRevenue
    FROM dbo.Orders o
    INNER JOIN dbo.OrderItems oi ON o.Id = oi.OrderId 
    INNER JOIN dbo.Medicines m ON oi.MedicineId = m.Id
    INNER JOIN dbo.MedicineCategories mc ON m.CategoryId = mc.Id
    WHERE o.CreatedAt >= @StartDate 
      AND o.CreatedAt <= @EndDate
      AND o.StatusId != 5 
    GROUP BY m.Name
);
GO