CREATE PROCEDURE [dbo].[GetSupplier]
  @SupplierId int
AS
begin
    select supplierid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone, fax, homepage 
    from Suppliers 
    where SupplierId = @SupplierId
end