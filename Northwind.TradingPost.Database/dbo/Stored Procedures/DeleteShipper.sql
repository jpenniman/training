create procedure DeleteShipper(@ShipperID int)
as
    delete from [Shippers] where [ShipperID] = @ShipperID;
GO
