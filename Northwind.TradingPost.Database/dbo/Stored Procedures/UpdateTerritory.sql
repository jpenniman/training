create procedure UpdateTerritory(@TerritoryID nvarchar(20),@TerritoryDescription nchar(50),@RegionID int)
as
    update [Territories]
    set [TerritoryDescription] = @TerritoryDescription,
        [RegionID] = @RegionID
    where TerritoryID = @TerritoryID;
GO
