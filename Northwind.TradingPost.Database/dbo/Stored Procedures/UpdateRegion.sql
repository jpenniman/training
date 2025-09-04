create procedure UpdateRegion(@RegionID int,@RegionDescription nchar(50))
as
    update [Region]
    set [RegionDescription] = @RegionDescription
    where RegionID = @RegionID;
GO
