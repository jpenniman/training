create procedure AddCustomerCustomerDemo(@CustomerID nchar(5),@CustomerTypeID nchar(10))
as
    insert into [CustomerCustomerDemo] ([CustomerID],[CustomerTypeID])
    values(@CustomerID,@CustomerTypeID);
GO
