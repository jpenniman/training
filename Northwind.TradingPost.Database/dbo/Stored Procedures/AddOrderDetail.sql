create procedure AddOrderDetail(@OrderID int,@ProductID int,@UnitPrice money,@Quantity smallint,@Discount real)
as
    insert into [Order Details] ([OrderID],[ProductID],[UnitPrice],[Quantity],[Discount])
    values(@OrderID,@ProductID,@UnitPrice,@Quantity,@Discount);
GO
