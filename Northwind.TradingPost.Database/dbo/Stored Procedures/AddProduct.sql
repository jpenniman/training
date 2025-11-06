create procedure AddProduct(@ProductID int out,@ProductName nvarchar(40),@SupplierID int,@CategoryID int,@QuantityPerUnit nvarchar(20),@UnitPrice money,@UnitsInStock smallint,@UnitsOnOrder smallint,@ReorderLevel smallint,@Discontinued bit)
as
    insert into [Products] ([ProductName],[SupplierID],[CategoryID],[QuantityPerUnit],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued])
    values(@ProductName,@SupplierID,@CategoryID,@QuantityPerUnit,@UnitPrice,@UnitsInStock,@UnitsOnOrder,@ReorderLevel,@Discontinued);

    select @ProductID = SCOPE_IDENTITY();
GO
