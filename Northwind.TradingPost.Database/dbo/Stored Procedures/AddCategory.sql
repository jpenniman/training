create procedure AddCategory(@CategoryID int out,
                             @CategoryName nvarchar(15),
                             @Description ntext = null,
                             @Picture image = null)
as
    if (@CategoryName is null)
        raiserror (N'CategoryName cannot be null.', 11, 1);
        
    insert into [Categories] ([CategoryName],[Description],[Picture])
    values(@CategoryName,@Description,@Picture);
    
    select @CategoryID = SCOPE_IDENTITY();
GO
