create procedure DeleteRegion(@RegionID int)
as
    delete from [Region] where [RegionID] = @RegionID;
GO
