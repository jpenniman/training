create procedure UpdateCustomerDemographic(@CustomerTypeID nchar(10),@CustomerDesc ntext)
as
    update [CustomerDemographics]
    set [CustomerDesc] = @CustomerDesc
    where CustomerTypeID = @CustomerTypeID;
GO
