create procedure UpdateCustomerCustomerDemo(@CustomerID nchar(5),@CustomerTypeID nchar(10))
as
    update [CustomerCustomerDemo]
    set [CustomerID] = @CustomerID,
        [CustomerTypeID] = @CustomerTypeID
    where [CustomerID] = @CustomerID and [CustomerTypeID] = @CustomerTypeID;
GO
