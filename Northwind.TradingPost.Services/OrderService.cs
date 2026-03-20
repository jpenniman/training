using Northwind.TradingPost.Common;
using Northwind.TradingPost.DataAccess;
using Northwind.TradingPost.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Services;

public class OrderService
{
    private OrderDAO _orderDao;
    private CustomerDAO _customerDao;
    private ProductDAO _productDao;
    private OrderDetailDAO _orderDetailDao;

    public OrderService()
    {
        _orderDao = new OrderDAO();
        _customerDao = new CustomerDAO();
        _productDao = new ProductDAO();
        _orderDetailDao = new OrderDetailDAO();
    }

    public bool CreateOrder(Order order, List<OrderDetail> details)
    {
        if (!ValidateOrder(order))
            return false;

        if (!ValidateInventory(details))
            return false;

        if (!_orderDao.Add(order))
            return false;

        foreach (var detail in details)
        {
            detail.OrderId = order.OrderId;
            _orderDetailDao.Add(detail);

            var product = _productDao.GetById(detail.ProductId);
            if (product != null)
            {
                product.UnitsInStock -= detail.Quantity;
                _productDao.Update(product);
            }
        }

        var customer = _customerDao.GetById(order.CustomerId);
        if (customer != null && !string.IsNullOrEmpty(customer.Email))
        {
            EmailHelper.SendOrderConfirmation(customer.Email, order.OrderId, CalculateOrderTotal(order, details));
        }

        return true;
    }

    public bool UpdateOrder(Order order)
    {
        var existing = _orderDao.GetById(order.OrderId);
        if (existing == null)
            return false;

        if (existing.OrderDate.HasValue && order.OrderDate.HasValue)
        {
            if (existing.OrderDate.Value != order.OrderDate.Value)
            {
                GlobalApplicationHelper.LogError(new Exception("Order date changed"), "OrderUpdate");
            }
        }

        return _orderDao.Update(order);
    }

    public bool CancelOrder(int orderId)
    {
        var order = _orderDao.GetById(orderId);
        if (order == null)
            return false;

        if (order.ShippedDate.HasValue)
        {
            return false;
        }

        var details = GetOrderDetails(orderId);
        foreach (var detail in details)
        {
            var product = _productDao.GetById(detail.ProductId);
            if (product != null)
            {
                product.UnitsInStock += detail.Quantity;
                _productDao.Update(product);
            }
        }

        return _orderDao.Delete(orderId);
    }

    public bool ValidateOrder(Order order)
    {
        if (order == null)
            return false;

        if (!GlobalApplicationHelper.IsValidCustomerId(order.CustomerId))
            return false;

        var customer = _customerDao.GetById(order.CustomerId);
        if (customer == null)
            return false;

        if (order.RequiredDate.HasValue && order.OrderDate.HasValue)
        {
            if (!GlobalApplicationHelper.ValidateOrderCanShip(order.OrderDate, order.RequiredDate))
                return false;
        }

        return true;
    }

    public bool ValidateInventory(List<OrderDetail> details)
    {
        foreach (var detail in details)
        {
            var product = _productDao.GetById(detail.ProductId);
            if (product == null)
                return false;

            if (product.UnitsInStock < detail.Quantity)
                return false;
        }
        return true;
    }

    public decimal CalculateOrderTotal(Order order, List<OrderDetail> details)
    {
        decimal total = 0;
        foreach (var detail in details)
        {
            var product = _productDao.GetById(detail.ProductId);
            if (product != null && product.UnitPrice.HasValue)
            {
                decimal unitPrice = product.UnitPrice.Value;
                decimal discount = (decimal)(1 - detail.Discount);
                total += unitPrice * detail.Quantity * discount;
            }
        }

        if (order.ShipCountry != null)
        {
            total += GlobalApplicationHelper.CalculateShippingCost(total, order.ShipCountry);
        }

        return total;
    }

    public DataTable GetOrdersByCustomer(string customerId)
    {
        return GlobalApplicationHelper.GetOrdersByCustomer(customerId);
    }

    public DataTable GetOrdersByDateRange(DateTime startDate, DateTime endDate)
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT * FROM Orders WHERE OrderDate BETWEEN @Start AND @End", cn);
            cmd.Parameters.AddWithValue("@Start", startDate);
            cmd.Parameters.AddWithValue("@End", endDate);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public List<OrderDetail> GetOrderDetails(int orderId)
    {
        var details = new List<OrderDetail>();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT od.*, p.ProductName 
                    FROM [Order Details] od
                    JOIN Products p ON od.ProductID = p.ProductID
                    WHERE od.OrderID = @OrderID", cn);
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            cn.Open();
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var detail = new OrderDetail();
                detail.OrderId = Convert.ToInt32(rdr["OrderID"]);
                detail.ProductId = Convert.ToInt32(rdr["ProductID"]);
                detail.ProductName = rdr["ProductName"].ToString();
                detail.UnitPrice = Convert.ToDecimal(rdr["UnitPrice"]);
                detail.Quantity = Convert.ToInt16(rdr["Quantity"]);
                detail.Discount = Convert.ToSingle(rdr["Discount"]);
                details.Add(detail);
            }
        }
        return details;
    }

    public (List<OrderDetail> Items, int TotalCount) GetOrderDetailsPaged(int orderId, int page, int pageSize)
    {
        var details = new List<OrderDetail>();
        int totalCount = 0;

        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var countCmd = new SqlCommand("SELECT COUNT(*) FROM [Order Details] WHERE OrderID = @OrderID", cn);
            countCmd.Parameters.AddWithValue("@OrderID", orderId);
            cn.Open();
            totalCount = (int)countCmd.ExecuteScalar();
            cn.Close();

            var cmd = new SqlCommand(@"
                    SELECT od.*, p.ProductName 
                    FROM [Order Details] od
                    JOIN Products p ON od.ProductID = p.ProductID
                    WHERE od.OrderID = @OrderID
                    ORDER BY od.ProductID
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", cn);
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cn.Open();
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var detail = new OrderDetail();
                detail.OrderId = Convert.ToInt32(rdr["OrderID"]);
                detail.ProductId = Convert.ToInt32(rdr["ProductID"]);
                detail.ProductName = rdr["ProductName"].ToString();
                detail.UnitPrice = Convert.ToDecimal(rdr["UnitPrice"]);
                detail.Quantity = Convert.ToInt16(rdr["Quantity"]);
                detail.Discount = Convert.ToSingle(rdr["Discount"]);
                details.Add(detail);
            }
        }
        return (details, totalCount);
    }

    public Order GetOrderById(int orderId)
    {
        return _orderDao.GetById(orderId);
    }

    public (List<Order> Items, int TotalCount) GetOrdersPaged(int page, int pageSize)
    {
        var orders = new List<Order>();
        int totalCount = 0;

        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var countCmd = new SqlCommand("SELECT COUNT(*) FROM Orders", cn);
            cn.Open();
            totalCount = (int)countCmd.ExecuteScalar();

            var cmd = new SqlCommand(@"
                    SELECT o.*, c.CompanyName as CustomerName
                    FROM Orders o
                    LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                    ORDER BY o.OrderDate DESC, o.OrderID DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", cn);
            cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var order = new Order();
                order.OrderId = Convert.ToInt32(rdr["OrderID"]);
                order.CustomerId = rdr["CustomerID"] != DBNull.Value ? rdr["CustomerID"].ToString() : null;
                order.EmployeeId = rdr["EmployeeID"] != DBNull.Value ? Convert.ToInt32(rdr["EmployeeID"]) : (int?)null;
                order.OrderDate = rdr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(rdr["OrderDate"]) : (DateTime?)null;
                order.RequiredDate = rdr["RequiredDate"] != DBNull.Value ? Convert.ToDateTime(rdr["RequiredDate"]) : (DateTime?)null;
                order.ShippedDate = rdr["ShippedDate"] != DBNull.Value ? Convert.ToDateTime(rdr["ShippedDate"]) : (DateTime?)null;
                order.Freight = rdr["Freight"] != DBNull.Value ? Convert.ToDecimal(rdr["Freight"]) : (decimal?)null;
                order.ShipName = rdr["ShipName"] != DBNull.Value ? rdr["ShipName"].ToString() : null;
                order.ShipCity = rdr["ShipCity"] != DBNull.Value ? rdr["ShipCity"].ToString() : null;
                order.ShipCountry = rdr["ShipCountry"] != DBNull.Value ? rdr["ShipCountry"].ToString() : null;
                orders.Add(order);
            }
        }
        return (orders, totalCount);
    }

    public bool ProcessShipment(int orderId, int shipperId, string trackingNumber)
    {
        var order = _orderDao.GetById(orderId);
        if (order == null)
            return false;

        if (order.ShippedDate.HasValue)
            return false;

        order.ShipVia = shipperId;
        order.ShippedDate = DateTime.Now;
            
        bool result = _orderDao.Update(order);

        if (result)
        {
            var customer = _customerDao.GetById(order.CustomerId);
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                EmailHelper.SendShipmentNotification(customer.Email, orderId, trackingNumber);
            }
        }

        return result;
    }

    public DataTable GetCustomerOrderHistory(string customerId)
    {
        return GlobalApplicationHelper.GetCustomerOrderHistory(customerId);
    }
}