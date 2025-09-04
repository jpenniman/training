create procedure AddRegion(@RegionID int,@RegionDescription nchar(50))
as
    insert into [Region] ([RegionID],[RegionDescription])
    values(@RegionID,@RegionDescription);
GO
