create procedure DeleteCustomerDemographic(@CustomerTypeID nchar(10))
as
    delete from [CustomerDemographics] where [CustomerTypeID] = @CustomerTypeID;
GO
