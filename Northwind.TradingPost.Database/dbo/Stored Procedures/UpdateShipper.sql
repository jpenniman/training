create procedure UpdateShipper(@ShipperID int,@CompanyName nvarchar(40),@Phone nvarchar(24))
as
    update [Shippers]
    set [CompanyName] = @CompanyName,
        [Phone] = @Phone
    where ShipperID = @ShipperID;
GO
