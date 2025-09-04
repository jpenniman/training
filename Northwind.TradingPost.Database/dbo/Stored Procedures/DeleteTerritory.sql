create procedure DeleteTerritory(@TerritoryID nvarchar(20))
as
    delete from [Territories] where [TerritoryID] = @TerritoryID;
GO
