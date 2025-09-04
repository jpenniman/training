create procedure UpdateSupplier(@SupplierID int,@CompanyName nvarchar(40),@ContactName nvarchar(30),@ContactTitle nvarchar(30),@Address nvarchar(60),@City nvarchar(15),@Region nvarchar(15),@PostalCode nvarchar(10),@Country nvarchar(15),@Phone nvarchar(24),@Fax nvarchar(24),@HomePage ntext)
as
    update [Suppliers]
    set [CompanyName] = @CompanyName,
        [ContactName] = @ContactName,
        [ContactTitle] = @ContactTitle,
        [Address] = @Address,
        [City] = @City,
        [Region] = @Region,
        [PostalCode] = @PostalCode,
        [Country] = @Country,
        [Phone] = @Phone,
        [Fax] = @Fax,
        [HomePage] = @HomePage
    where SupplierID = @SupplierID;
GO
