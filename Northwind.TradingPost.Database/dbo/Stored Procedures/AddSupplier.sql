create procedure AddSupplier(@SupplierID int out,@CompanyName nvarchar(40),@ContactName nvarchar(30),@ContactTitle nvarchar(30),@Address nvarchar(60),@City nvarchar(15),@Region nvarchar(15),@PostalCode nvarchar(10),@Country nvarchar(15),@Phone nvarchar(24),@Fax nvarchar(24),@HomePage ntext)
as
    insert into [Suppliers] ([CompanyName],[ContactName],[ContactTitle],[Address],[City],[Region],[PostalCode],[Country],[Phone],[Fax],[HomePage])
    values(@CompanyName,@ContactName,@ContactTitle,@Address,@City,@Region,@PostalCode,@Country,@Phone,@Fax,@HomePage);

    select @SupplierID = SCOPE_IDENTITY();
GO
