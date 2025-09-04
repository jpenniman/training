create procedure UpdateOrderDetail(@OrderID int,@ProductID int,@UnitPrice money,@Quantity smallint,@Discount real)
as
    update [Order Details]
    set [UnitPrice] = @UnitPrice,
        [Quantity] = @Quantity,
        [Discount] = @Discount
    where OrderID = @OrderID and ProductID = @ProductID;
GO
