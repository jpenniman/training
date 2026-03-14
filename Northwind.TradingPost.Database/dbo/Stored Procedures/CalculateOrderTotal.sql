create procedure CalculateOrderTotal
    @OrderID int,
    @SubTotal money out,
    @Discount money out,
    @Shipping money out,
    @Total money out
as
begin
    select @SubTotal = SUM(UnitPrice * Quantity)
    from [Order Details]
    where OrderID = @OrderID
    
    select @Discount = SUM(UnitPrice * Quantity * Discount)
    from [Order Details]
    where OrderID = @OrderID
    
    set @SubTotal = ISNULL(@SubTotal, 0)
    set @Discount = ISNULL(@Discount, 0)
    
    declare @OrderTotal money
    set @OrderTotal = @SubTotal - @Discount
    
    declare @ShipCountry nvarchar(15)
    select @ShipCountry = ShipCountry from Orders where OrderID = @OrderID
    
    if @OrderTotal > 1000
        set @Shipping = 0
    else if @ShipCountry = 'USA'
        set @Shipping = @OrderTotal * 0.05
    else
        set @Shipping = @OrderTotal * 0.15
    
    set @Total = @OrderTotal + @Shipping
end
