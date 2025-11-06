create procedure AddShipper(@ShipperID int out,@CompanyName nvarchar(40),@Phone nvarchar(24))
as
    insert into [Shippers] ([CompanyName],[Phone])
    values(@CompanyName,@Phone);
    
    select @ShipperID = SCOPE_IDENTITY();
GO
