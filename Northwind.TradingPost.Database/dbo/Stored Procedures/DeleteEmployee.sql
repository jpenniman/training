create procedure DeleteEmployee(@EmployeeID int)
as
    delete from [Employees] where [EmployeeID] = @EmployeeID;
GO
