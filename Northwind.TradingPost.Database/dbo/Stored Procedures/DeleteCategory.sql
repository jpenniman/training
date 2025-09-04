create procedure DeleteCategory(@CategoryID int)
as
    delete from [Categories] where [CategoryID] = @CategoryID;
GO
