create procedure AddCustomer(@CustomerID nchar(5) out,
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
begin
    declare @result int = 2627, @attempt tinyint = 0; 
    /* We want to try 3 times if the insert fails with a duplicate key */
    while @result = 2627 and @attempt < 3
        begin
            /* Generate a random 5 character CustomerID */
            /* round(rand() * 25,0) produces a random integer between 0 and 25 */
            /* Adding 65 gives a value between 65 and 90, the ASCII values for A-Z */
            select @CustomerID = (
                 char((round(rand() * 25,0) + 65))
                +char((round(rand() * 25,0) + 65))
                +char((round(rand() * 25,0) + 65))
                +char((round(rand() * 25,0) + 65))
                +char((round(rand() * 25,0) + 65)));

            insert into [Customers] ([CustomerID],[CompanyName],[ContactName],[ContactTitle],[Address],[City],[Region],[PostalCode],[Country],[Phone],[Fax])
            values(@CustomerID,@CompanyName,@ContactName,@ContactTitle,@Address,@City,@Region,@PostalCode,@Country,@Phone,@Fax);

            set @result = @@error;
            set @attempt = @attempt + 1;
        end
end
GO
