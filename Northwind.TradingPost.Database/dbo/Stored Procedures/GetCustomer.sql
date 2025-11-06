CREATE PROCEDURE [dbo].[GetCustomer]
  @CustomerID CustomerIDType
AS
begin
  select CustomerID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax
  from Customers
  where CustomerID = @CustomerID;
end
go