create procedure DeleteCustomerCustomerDemo(@CustomerID nchar(5),@CustomerTypeID nchar(10))
as
    delete from [CustomerCustomerDemo] where [CustomerID] = @CustomerID and [CustomerTypeID] = @CustomerTypeID;
GO
