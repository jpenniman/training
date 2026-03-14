create procedure AddOrderDetailWithValidation
    @OrderID int,
    @ProductID int,
    @UnitPrice money,
    @Quantity smallint,
    @Discount real,
    @ErrorMessage nvarchar(500) out
as
begin
    set @ErrorMessage = ''
    
    if not exists (select 1 from Orders where OrderID = @OrderID)
    begin
        set @ErrorMessage = 'Order does not exist'
        return -1
    end
    
    if not exists (select 1 from Products where ProductID = @ProductID)
    begin
        set @ErrorMessage = 'Product does not exist'
        return -2
    end
    
    declare @UnitsInStock smallint
    select @UnitsInStock = UnitsInStock from Products where ProductID = @ProductID
    
    if @UnitsInStock < @Quantity
    begin
        set @ErrorMessage = 'Insufficient inventory. Available: ' + cast(@UnitsInStock as nvarchar(10))
        return -3
    end
    
    if @UnitPrice < 0
    begin
        set @ErrorMessage = 'Unit price cannot be negative'
        return -4
    end
    
    if @Quantity <= 0
    begin
        set @ErrorMessage = 'Quantity must be greater than zero'
        return -5
    end
    
    if @Discount < 0 or @Discount > 1
    begin
        set @ErrorMessage = 'Discount must be between 0 and 1'
        return -6
    end
    
    insert into [Order Details] ([OrderID],[ProductID],[UnitPrice],[Quantity],[Discount])
    values(@OrderID,@ProductID,@UnitPrice,@Quantity,@Discount)
    
    update Products set UnitsInStock = UnitsInStock - @Quantity where ProductID = @ProductID
    
    return 0
end
