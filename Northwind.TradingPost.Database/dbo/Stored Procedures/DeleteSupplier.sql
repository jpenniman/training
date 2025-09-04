create procedure DeleteSupplier(@SupplierID int)
as
    delete from [Suppliers] where [SupplierID] = @SupplierID;
GO
