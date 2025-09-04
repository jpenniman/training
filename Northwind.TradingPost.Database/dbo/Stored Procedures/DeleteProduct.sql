create procedure DeleteProduct(@ProductID int)
as
    delete from [Products] where [ProductID] = @ProductID;
GO
