create procedure AddOrder(@OrderID int out,@CustomerID nchar(5),@EmployeeID int,@OrderDate datetime,@RequiredDate datetime,@ShipVia int,@Freight money,@ShipName nvarchar(40),@ShipAddress nvarchar(60),@ShipCity nvarchar(15),@ShipRegion nvarchar(15),@ShipPostalCode nvarchar(10),@ShipCountry nvarchar(15))
as
    insert into [Orders] ([CustomerID],[EmployeeID],[OrderDate],[RequiredDate],[ShipVia],[Freight],[ShipName],[ShipAddress],[ShipCity],[ShipRegion],[ShipPostalCode],[ShipCountry])
    values(@CustomerID,@EmployeeID,@OrderDate,@RequiredDate,@ShipVia,@Freight,@ShipName,@ShipAddress,@ShipCity,@ShipRegion,@ShipPostalCode,@ShipCountry);
    
    select @OrderID = SCOPE_IDENTITY();
GO
