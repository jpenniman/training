create procedure DeleteOrderDetail(@OrderID int, @ProductID int)
as
    delete from [Order Details] where [OrderID] = @OrderID and [ProductID] = @ProductID;
GO
