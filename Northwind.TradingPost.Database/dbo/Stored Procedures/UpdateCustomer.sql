create procedure UpdateCustomer(@CustomerID nchar(5),
                                @CompanyName nvarchar(40),
                                @ContactName nvarchar(30) = null,
                                @ContactTitle nvarchar(30) = null,
                                @Address nvarchar(60) = null,
                                @City nvarchar(15) = null,
                                @Region nvarchar(15) = null,
                                @PostalCode nvarchar(10) = null,
                                @Country nvarchar(15) = null,
                                @Phone nvarchar(24) = null,
                                @Fax nvarchar(24) = null)
as
    update [Customers]
    set [CompanyName] = @CompanyName,
        [ContactName] = @ContactName,
        [ContactTitle] = @ContactTitle,
        [Address] = @Address,
        [City] = @City,
        [Region] = @Region,
        [PostalCode] = @PostalCode,
        [Country] = @Country,
        [Phone] = @Phone,
        [Fax] = @Fax
    where CustomerID = @CustomerID;
GO
