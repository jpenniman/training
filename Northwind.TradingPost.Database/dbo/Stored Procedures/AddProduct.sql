create procedure AddProduct(@ProductID int,@ProductName nvarchar(40),@SupplierID int,@CategoryID int,@QuantityPerUnit nvarchar(20),@UnitPrice money,@UnitsInStock smallint,@UnitsOnOrder smallint,@ReorderLevel smallint,@Discontinued bit)
as
    insert into [Products] ([ProductID],[ProductName],[SupplierID],[CategoryID],[QuantityPerUnit],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued])
    values(@ProductID,@ProductName,@SupplierID,@CategoryID,@QuantityPerUnit,@UnitPrice,@UnitsInStock,@UnitsOnOrder,@ReorderLevel,@Discontinued);
GO
