create procedure UpdateEmployee(@EmployeeID int,@LastName nvarchar(20),@FirstName nvarchar(10),@Title nvarchar(30),@TitleOfCourtesy nvarchar(25),@BirthDate datetime,@HireDate datetime,@Address nvarchar(60),@City nvarchar(15),@Region nvarchar(15),@PostalCode nvarchar(10),@Country nvarchar(15),@HomePhone nvarchar(24),@Extension nvarchar(4),@Photo image,@Notes ntext,@ReportsTo int,@PhotoPath nvarchar(255))
as
    update [Employees]
    set [LastName] = @LastName,
        [FirstName] = @FirstName,
        [Title] = @Title,
        [TitleOfCourtesy] = @TitleOfCourtesy,
        [BirthDate] = @BirthDate,
        [HireDate] = @HireDate,
        [Address] = @Address,
        [City] = @City,
        [Region] = @Region,
        [PostalCode] = @PostalCode,
        [Country] = @Country,
        [HomePhone] = @HomePhone,
        [Extension] = @Extension,
        [Photo] = @Photo,
        [Notes] = @Notes,
        [ReportsTo] = @ReportsTo,
        [PhotoPath] = @PhotoPath
    where EmployeeID = @EmployeeID;
GO
