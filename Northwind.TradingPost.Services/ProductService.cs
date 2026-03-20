using Northwind.TradingPost.Common;
using Northwind.TradingPost.DataAccess;
using Northwind.TradingPost.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Services;

public class ProductService
{
    private ProductDAO _productDao;

    public ProductService()
    {
        _productDao = new ProductDAO();
    }

    public bool AddProduct(Product product)
    {
        if (!ValidateProduct(product))
            return false;

        return _productDao.Add(product);
    }

    public bool UpdateProduct(Product product)
    {
        var existing = _productDao.GetById(product.ProductId);
        if (existing == null)
            return false;

        bool result = _productDao.Update(product);

        if (result && product.UnitsInStock < 10)
        {
            EmailHelper.SendLowStockAlert(product.ProductName, (int)product.UnitsInStock);
        }

        return result;
    }

    public bool DeleteProduct(int productId)
    {
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT COUNT(*) FROM [Order Details] WHERE ProductID = @ProductID", cn);
            cmd.Parameters.AddWithValue("@ProductID", productId);
            cn.Open();
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0)
                return false;
        }

        return _productDao.Delete(productId);
    }

    public bool ValidateProduct(Product product)
    {
        if (product == null)
            return false;

        if (string.IsNullOrWhiteSpace(product.ProductName))
            return false;

        if (!product.UnitPrice.HasValue || product.UnitPrice.Value < 0)
            return false;

        if (product.UnitsInStock < 0)
            return false;

        if (!product.SupplierId.HasValue && !product.CategoryId.HasValue)
            return false;

        return true;
    }

    public bool UpdateStock(int productId, int quantityChange)
    {
        return GlobalApplicationHelper.UpdateProductStock(productId, quantityChange);
    }

    public List<Product> GetProductsByCategory(int categoryId)
    {
        var products = new List<Product>();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT * FROM Products WHERE CategoryID = @CategoryID", cn);
            cmd.Parameters.AddWithValue("@CategoryID", categoryId);
            cn.Open();
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var product = new Product();
                product.ProductId = Convert.ToInt32(rdr["ProductID"]);
                product.ProductName = rdr["ProductName"].ToString();
                product.SupplierId = rdr["SupplierID"] != DBNull.Value ? Convert.ToInt32(rdr["SupplierID"]) : (int?)null;
                product.CategoryId = rdr["CategoryID"] != DBNull.Value ? Convert.ToInt32(rdr["CategoryID"]) : (int?)null;
                product.QuantityPerUnit = rdr["QuantityPerUnit"] != DBNull.Value ? rdr["QuantityPerUnit"].ToString() : null;
                product.UnitPrice = rdr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(rdr["UnitPrice"]) : (decimal?)null;
                product.UnitsInStock = Convert.ToInt16(rdr["UnitsInStock"]);
                product.UnitsOnOrder = Convert.ToInt16(rdr["UnitsOnOrder"]);
                product.ReorderLevel = Convert.ToInt16(rdr["ReorderLevel"]);
                product.Discontinued = Convert.ToBoolean(rdr["Discontinued"]);
                products.Add(product);
            }
        }
        return products;
    }

    public List<Product> GetLowStockProducts(int threshold = 10)
    {
        var products = new List<Product>();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT * FROM Products WHERE UnitsInStock < @Threshold AND Discontinued = 0", cn);
            cmd.Parameters.AddWithValue("@Threshold", threshold);
            cn.Open();
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var product = new Product();
                product.ProductId = Convert.ToInt32(rdr["ProductID"]);
                product.ProductName = rdr["ProductName"].ToString();
                product.UnitsInStock = Convert.ToInt16(rdr["UnitsInStock"]);
                products.Add(product);
            }
        }
        return products;
    }

    public List<string> GetTop10ExpensiveProducts()
    {
        return GlobalApplicationHelper.GetTop10ExpensiveProducts();
    }

    public Product GetProductById(int productId)
    {
        return _productDao.GetById(productId);
    }

    public (List<Product> Items, int TotalCount) GetProductsPaged(int page, int pageSize)
    {
        var products = new List<Product>();
        int totalCount = 0;

        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var countCmd = new SqlCommand("SELECT COUNT(*) FROM Products", cn);
            cn.Open();
            totalCount = (int)countCmd.ExecuteScalar();

            var cmd = new SqlCommand(@"
                    SELECT p.*, c.CategoryName, s.CompanyName as SupplierName
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                    LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                    ORDER BY p.ProductName
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", cn);
            cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var product = new Product();
                product.ProductId = Convert.ToInt32(rdr["ProductID"]);
                product.ProductName = rdr["ProductName"].ToString();
                product.SupplierId = rdr["SupplierID"] != DBNull.Value ? Convert.ToInt32(rdr["SupplierID"]) : (int?)null;
                product.CategoryId = rdr["CategoryID"] != DBNull.Value ? Convert.ToInt32(rdr["CategoryID"]) : (int?)null;
                product.QuantityPerUnit = rdr["QuantityPerUnit"] != DBNull.Value ? rdr["QuantityPerUnit"].ToString() : null;
                product.UnitPrice = rdr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(rdr["UnitPrice"]) : (decimal?)null;
                product.UnitsInStock = Convert.ToInt16(rdr["UnitsInStock"]);
                product.UnitsOnOrder = Convert.ToInt16(rdr["UnitsOnOrder"]);
                product.ReorderLevel = Convert.ToInt16(rdr["ReorderLevel"]);
                product.Discontinued = Convert.ToBoolean(rdr["Discontinued"]);
                products.Add(product);
            }
        }
        return (products, totalCount);
    }

    public List<Category> GetCategories()
    {
        var categoryDao = new CategoryDAO();
        return categoryDao.GetAll();
    }
}