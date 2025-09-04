create procedure UpdateCategory(@CategoryID int,@CategoryName nvarchar(15),@Description ntext,@Picture image)
as
    update [Categories]
    set [CategoryName] = @CategoryName,
        [Description] = @Description,
        [Picture] = @Picture
    where CategoryID = @CategoryID;
GO
