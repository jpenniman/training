create procedure DeleteEmployeeTerritory(@EmployeeID int,@TerritoryID nvarchar(20))
as
    delete from [EmployeeTerritories] where [EmployeeID] = @EmployeeID and [TerritoryID] = @TerritoryID;
GO
