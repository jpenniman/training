using Northwind.TradingPost.Common;
using Northwind.TradingPost.Domain;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Services;

public class ShippingService
{
    public bool ValidateShippingInfo(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.ShipName))
            return false;

        if (string.IsNullOrWhiteSpace(order.ShipAddress))
            return false;

        if (string.IsNullOrWhiteSpace(order.ShipCity))
            return false;

        if (string.IsNullOrWhiteSpace(order.ShipCountry))
            return false;

        if (!string.IsNullOrWhiteSpace(order.ShipRegion))
        {
            if (!ValidationHelper.IsValidRegion(order.ShipRegion))
                return false;
        }

        return true;
    }

    public decimal CalculateShippingCost(decimal orderTotal, string shipCountry)
    {
        return GlobalApplicationHelper.CalculateShippingCost(orderTotal, shipCountry);
    }

    public bool ProcessShipment(int orderId, int shipperId)
    {
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    UPDATE Orders 
                    SET ShippedDate = GETDATE(), ShipVia = @ShipperID 
                    WHERE OrderID = @OrderID AND ShippedDate IS NULL", cn);
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            cmd.Parameters.AddWithValue("@ShipperID", shipperId);
                
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public DataTable GetOrdersReadyToShip()
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT o.*, c.CompanyName as CustomerName
                    FROM Orders o
                    JOIN Customers c ON o.CustomerID = c.CustomerID
                    WHERE o.ShippedDate IS NULL 
                    AND o.RequiredDate IS NOT NULL
                    AND o.RequiredDate <= DATEADD(day, 3, GETDATE())", cn);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public DataTable GetShippedOrdersByDate(DateTime shipDate)
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT o.*, s.CompanyName as ShipperName
                    FROM Orders o
                    JOIN Shippers s ON o.ShipVia = s.ShipperID
                    WHERE CAST(o.ShippedDate AS DATE) = @ShipDate", cn);
            cmd.Parameters.AddWithValue("@ShipDate", shipDate.Date);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }
}