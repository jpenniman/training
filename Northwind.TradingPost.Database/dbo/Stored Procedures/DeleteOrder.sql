create procedure DeleteOrder(@OrderID int)
as
    delete from [Order Details] where [OrderID] = @OrderID;
    delete from [Orders] where [OrderID] = @OrderID;
GO
