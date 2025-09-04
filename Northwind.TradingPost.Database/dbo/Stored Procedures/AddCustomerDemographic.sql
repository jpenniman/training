create procedure AddCustomerDemographic(@CustomerTypeID nchar(10),@CustomerDesc ntext)
as
    insert into [CustomerDemographics] ([CustomerTypeID],[CustomerDesc])
    values(@CustomerTypeID,@CustomerDesc);
GO
