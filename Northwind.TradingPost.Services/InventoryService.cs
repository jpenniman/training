using Northwind.TradingPost.Common;
using Northwind.TradingPost.DataAccess;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Services;

public class InventoryService
{
    private ProductDAO _productDao;

    public InventoryService()
    {
        _productDao = new ProductDAO();
    }

    public bool AdjustStock(int productId, int newQuantity, string reason)
    {
        var product = _productDao.GetById(productId);
        if (product == null)
            return false;

        int change = newQuantity - (int)product.UnitsInStock;
        product.UnitsInStock = (short)newQuantity;

        bool result = _productDao.Update(product);

        if (result && product.UnitsInStock < 10)
        {
            EmailHelper.SendLowStockAlert(product.ProductName, (int)product.UnitsInStock);
        }

        return result;
    }

    public bool ReorderProduct(int productId)
    {
        var product = _productDao.GetById(productId);
        if (product == null)
            return false;

        int reorderQty = product.ReorderLevel.HasValue && product.ReorderLevel > 0 ? (int)product.ReorderLevel * 2 : 10;
        product.UnitsOnOrder += (short)reorderQty;

        return _productDao.Update(product);
    }

    public DataTable GetInventoryReport()
    {
        return new ReportingService().GetInventoryValueReport();
    }

    public DataTable GetDiscontinuedProducts()
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT * FROM Products WHERE Discontinued = 1", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public bool DiscontinueProduct(int productId)
    {
        var product = _productDao.GetById(productId);
        if (product == null)
            return false;

        product.Discontinued = true;
        return _productDao.Update(product);
    }
}