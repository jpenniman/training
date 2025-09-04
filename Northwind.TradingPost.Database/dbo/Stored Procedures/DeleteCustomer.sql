create procedure DeleteCustomer(@CustomerID nchar(5))
as
    delete from [Customers] where [CustomerID] = @CustomerID;
GO
