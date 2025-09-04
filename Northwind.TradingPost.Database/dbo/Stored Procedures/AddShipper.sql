create procedure AddShipper(@ShipperID int,@CompanyName nvarchar(40),@Phone nvarchar(24))
as
    insert into [Shippers] ([ShipperID],[CompanyName],[Phone])
    values(@ShipperID,@CompanyName,@Phone);
GO
