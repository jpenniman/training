create procedure AddTerritory(@TerritoryID nvarchar(20),@TerritoryDescription nchar(50),@RegionID int)
as
    insert into [Territories] ([TerritoryID],[TerritoryDescription],[RegionID])
    values(@TerritoryID,@TerritoryDescription,@RegionID);
GO
