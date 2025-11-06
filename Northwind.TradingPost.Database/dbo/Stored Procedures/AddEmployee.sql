create procedure AddEmployee(@EmployeeID int out,@LastName nvarchar(20),@FirstName nvarchar(10),@Title nvarchar(30),@TitleOfCourtesy nvarchar(25),@BirthDate datetime,@HireDate datetime,@Address nvarchar(60),@City nvarchar(15),@Region nvarchar(15),@PostalCode nvarchar(10),@Country nvarchar(15),@HomePhone nvarchar(24),@Extension nvarchar(4),@Photo image,@Notes ntext,@ReportsTo int,@PhotoPath nvarchar(255))
as
    insert into [Employees] ([LastName],[FirstName],[Title],[TitleOfCourtesy],[BirthDate],[HireDate],[Address],[City],[Region],[PostalCode],[Country],[HomePhone],[Extension],[Photo],[Notes],[ReportsTo],[PhotoPath])
    values(@LastName,@FirstName,@Title,@TitleOfCourtesy,@BirthDate,@HireDate,@Address,@City,@Region,@PostalCode,@Country,@HomePhone,@Extension,@Photo,@Notes,@ReportsTo,@PhotoPath);

    select @EmployeeID = SCOPE_IDENTITY();
GO
