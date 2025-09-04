create procedure UpdateEmployeeTerritory(@EmployeeID int,@TerritoryID nvarchar(20))
as
    update [EmployeeTerritories]
    set [EmployeeID] = @EmployeeID,
        [TerritoryID] = @TerritoryID
    where [EmployeeID] = @EmployeeID and [TerritoryID] = @TerritoryID;
GO
