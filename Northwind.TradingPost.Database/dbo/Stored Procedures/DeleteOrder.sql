create procedure DeleteOrder(@OrderID int)
as
    begin tran
    begin try
        delete from [Order Details] where [OrderID] = @OrderID;
        delete from [Orders] where [OrderID] = @OrderID;
        commit tran;
    end try
    begin catch
        rollback tran;
    end catch;
GO
