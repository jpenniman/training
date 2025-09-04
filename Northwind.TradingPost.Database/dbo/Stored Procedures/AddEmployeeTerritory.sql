create procedure AddEmployeeTerritory(@EmployeeID int,@TerritoryID nvarchar(20))
as
    insert into [EmployeeTerritories] ([EmployeeID],[TerritoryID])
    values(@EmployeeID,@TerritoryID);
GO
