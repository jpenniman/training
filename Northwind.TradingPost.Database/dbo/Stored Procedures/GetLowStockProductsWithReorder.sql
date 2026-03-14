create procedure GetLowStockProductsWithReorder
    @Threshold smallint = 10
as
begin
    select 
        p.ProductID,
        p.ProductName,
        p.UnitsInStock,
        p.UnitsOnOrder,
        p.ReorderLevel,
        p.SupplierID,
        s.CompanyName as SupplierName,
        case 
            when p.UnitsInStock <= p.ReorderLevel then 'REORDER NEEDED'
            when p.UnitsInStock < @Threshold then 'LOW STOCK'
            else 'OK'
        end as StockStatus
    from Products p
    left join Suppliers s on p.SupplierID = s.SupplierID
    where p.Discontinued = 0
    and (p.UnitsInStock < @Threshold OR p.UnitsInStock <= p.ReorderLevel)
    order by p.UnitsInStock asc
end
