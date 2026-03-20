using Northwind.TradingPost.Common;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Services;

public class ReportingService
{
    public DataTable GetSalesByYearReport(DateTime? startDate, DateTime? endDate)
    {
        return GlobalApplicationHelper.GetSalesByYear(startDate, endDate);
    }

    public DataTable GetSalesByCategoryReport(string categoryName, string ordYear)
    {
        return GlobalApplicationHelper.GetSalesByCategory(categoryName, ordYear);
    }

    public DataTable GetCustomerOrdersReport(string customerId)
    {
        return GlobalApplicationHelper.GetCustomerOrderHistory(customerId);
    }

    public DataTable GetTopProductsReport(int topN = 10)
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand($@"
                    SELECT TOP {topN} p.ProductName, SUM(od.Quantity) as TotalQuantity, SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) as TotalSales
                    FROM [Order Details] od
                    JOIN Products p ON od.ProductID = p.ProductID
                    JOIN Orders o ON od.OrderID = o.OrderID
                    WHERE o.OrderDate IS NOT NULL
                    GROUP BY p.ProductName
                    ORDER BY TotalSales DESC", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public DataTable GetEmployeeSalesReport(int? employeeId, DateTime? startDate, DateTime? endDate)
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var sql = @"
                    SELECT e.FirstName + ' ' + e.LastName as EmployeeName, 
                           COUNT(o.OrderID) as OrderCount, 
                           SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) as TotalSales
                    FROM Employees e
                    JOIN Orders o ON e.EmployeeID = o.EmployeeID
                    JOIN [Order Details] od ON o.OrderID = od.OrderID
                    WHERE 1=1";
                
            if (employeeId.HasValue)
                sql += $" AND e.EmployeeID = {employeeId.Value}";
            if (startDate.HasValue)
                sql += $" AND o.OrderDate >= '{startDate.Value:yyyy-MM-dd}'";
            if (endDate.HasValue)
                sql += $" AND o.OrderDate <= '{endDate.Value:yyyy-MM-dd}'";
                
            sql += " GROUP BY e.FirstName, e.LastName";

            var cmd = new SqlCommand(sql, cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public DataTable GetCategorySalesReport()
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT c.CategoryName, 
                           COUNT(DISTINCT p.ProductID) as ProductCount,
                           SUM(od.Quantity) as TotalQuantity,
                           SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) as TotalSales
                    FROM Categories c
                    JOIN Products p ON c.CategoryID = p.CategoryID
                    JOIN [Order Details] od ON p.ProductID = od.ProductID
                    GROUP BY c.CategoryName
                    ORDER BY TotalSales DESC", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public DataTable GetShipperPerformanceReport()
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT s.CompanyName, 
                           COUNT(o.OrderID) as OrderCount,
                           SUM(o.Freight) as TotalFreight
                    FROM Shippers s
                    JOIN Orders o ON s.ShipperID = o.ShipVia
                    GROUP BY s.CompanyName
                    ORDER BY OrderCount DESC", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public DataTable GetCustomerLifetimeValueReport()
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT c.CustomerID, c.CompanyName,
                           COUNT(o.OrderID) as OrderCount,
                           SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) as LifetimeValue
                    FROM Customers c
                    JOIN Orders o ON c.CustomerID = o.CustomerID
                    JOIN [Order Details] od ON o.OrderID = od.OrderID
                    GROUP BY c.CustomerID, c.CompanyName
                    ORDER BY LifetimeValue DESC", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public DataTable GetInventoryValueReport()
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT p.ProductName, c.CategoryName, s.CompanyName as SupplierName,
                           p.UnitsInStock, p.UnitPrice, 
                           p.UnitsInStock * p.UnitPrice as InventoryValue
                    FROM Products p
                    JOIN Categories c ON p.CategoryID = c.CategoryID
                    JOIN Suppliers s ON p.SupplierID = s.SupplierID
                    WHERE p.Discontinued = 0
                    ORDER BY InventoryValue DESC", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public DataTable GetMonthlySalesTrendReport(int year)
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand($@"
                    SELECT MONTH(o.OrderDate) as Month, 
                           SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) as MonthlySales
                    FROM Orders o
                    JOIN [Order Details] od ON o.OrderID = od.OrderID
                    WHERE YEAR(o.OrderDate) = {year}
                    GROUP BY MONTH(o.OrderDate)
                    ORDER BY Month", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }
}