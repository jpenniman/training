CREATE PROCEDURE [dbo].[GetShipper]
  @ShipperId int
AS
begin
    select ShipperId, CompanyName, Phone from Shippers where ShipperId = @ShipperId;
end
go