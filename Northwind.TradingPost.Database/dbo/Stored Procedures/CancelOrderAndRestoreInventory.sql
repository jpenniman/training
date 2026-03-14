create procedure CancelOrderAndRestoreInventory
    @OrderID int,
    @Success bit out,
    @ErrorMessage nvarchar(500) out
as
begin
    set @Success = 0
    set @ErrorMessage = ''
    
    if not exists (select 1 from Orders where OrderID = @OrderID)
    begin
        set @ErrorMessage = 'Order does not exist'
        return -1
    end
    
    if exists (select 1 from Orders where OrderID = @OrderID and ShippedDate is not null)
    begin
        set @ErrorMessage = 'Cannot cancel shipped order'
        return -2
    end
    
    update Products 
    set UnitsInStock = UnitsInStock + od.Quantity
    from Products p
    join [Order Details] od on p.ProductID = od.ProductID
    where od.OrderID = @OrderID
    
    delete from [Order Details] where OrderID = @OrderID
    delete from Orders where OrderID = @OrderID
    
    set @Success = 1
    return 0
end
