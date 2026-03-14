create procedure ProcessOrderWithInventoryCheck
    @CustomerID nchar(5),
    @EmployeeID int,
    @OrderDate datetime,
    @RequiredDate datetime,
    @ShipVia int,
    @Freight money,
    @ShipName nvarchar(40),
    @ShipAddress nvarchar(60),
    @ShipCity nvarchar(15),
    @ShipRegion nvarchar(15),
    @ShipPostalCode nvarchar(10),
    @ShipCountry nvarchar(15),
    @OrderID int out,
    @ErrorMessage nvarchar(500) out
as
begin
    set @ErrorMessage = ''
    
    if not exists (select 1 from Customers where CustomerID = @CustomerID)
    begin
        set @ErrorMessage = 'Invalid customer ID'
        return -1
    end
    
    if @RequiredDate < @OrderDate
    begin
        set @ErrorMessage = 'Required date cannot be before order date'
        return -2
    end
    
    insert into [Orders] ([CustomerID],[EmployeeID],[OrderDate],[RequiredDate],[ShipVia],[Freight],[ShipName],[ShipAddress],[ShipCity],[ShipRegion],[ShipPostalCode],[ShipCountry])
    values(@CustomerID,@EmployeeID,@OrderDate,@RequiredDate,@ShipVia,@Freight,@ShipName,@ShipAddress,@ShipCity,@ShipRegion,@ShipPostalCode,@ShipCountry)
    
    select @OrderID = SCOPE_IDENTITY()
    
    return 0
end
