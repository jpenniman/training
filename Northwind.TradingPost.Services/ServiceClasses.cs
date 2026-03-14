using Northwind.TradingPost.Domain;
using Northwind.TradingPost.DataAccess;
using Northwind.TradingPost.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Northwind.TradingPost.Services
{
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

    public class CustomerService
    {
        private CustomerDAO _customerDao;

        public CustomerService()
        {
            _customerDao = new CustomerDAO();
        }

        public bool AddCustomer(Customer customer)
        {
            if (!ValidateCustomer(customer))
                return false;

            bool result = _customerDao.Add(customer);

            if (result && !string.IsNullOrEmpty(customer.Email))
            {
                EmailHelper.SendWelcomeEmail(customer.Email, customer.CompanyName);
            }

            return result;
        }

        public bool UpdateCustomer(Customer customer)
        {
            var existing = _customerDao.GetById(customer.CustomerId);
            if (existing == null)
                return false;

            return _customerDao.Update(customer);
        }

        public bool DeleteCustomer(string customerId)
        {
            var orders = GlobalApplicationHelper.GetOrdersByCustomer(customerId);
            if (orders.Rows.Count > 0)
            {
                GlobalApplicationHelper.LogError(new Exception($"Cannot delete customer {customerId} with existing orders"), "DeleteCustomer");
                return false;
            }

            return GlobalApplicationHelper.DeleteCustomer(customerId);
        }

        public bool ValidateCustomer(Customer customer)
        {
            if (customer == null)
                return false;

            if (string.IsNullOrWhiteSpace(customer.CustomerId))
                return false;

            if (customer.CustomerId.Length != 5)
                return false;

            if (!string.IsNullOrEmpty(customer.Email))
            {
                if (!GlobalApplicationHelper.IsValidEmail(customer.Email))
                    return false;
            }

            return true;
        }

        public bool ApplyDiscount(string customerId, decimal discountPercent)
        {
            return GlobalApplicationHelper.ApplyDiscount(customerId, discountPercent);
        }

        public string GetCustomerDiscount(string customerId)
        {
            return GlobalApplicationHelper.GetCustomerDiscount(customerId);
        }

        public List<Customer> GetCustomersByCountry(string country)
        {
            var customers = new List<Customer>();
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM Customers WHERE Country = @Country", cn);
                cmd.Parameters.AddWithValue("@Country", country);
                cn.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var customer = new Customer();
                    customer.CustomerId = rdr["CustomerID"].ToString();
                    customer.CompanyName = rdr["CompanyName"].ToString();
                    customer.ContactName = rdr["ContactName"] != DBNull.Value ? rdr["ContactName"].ToString() : null;
                    customer.ContactTitle = rdr["ContactTitle"] != DBNull.Value ? rdr["ContactTitle"].ToString() : null;
                    customer.Address = rdr["Address"] != DBNull.Value ? rdr["Address"].ToString() : null;
                    customer.City = rdr["City"] != DBNull.Value ? rdr["City"].ToString() : null;
                    customer.Region = rdr["Region"] != DBNull.Value ? rdr["Region"].ToString() : null;
                    customer.PostalCode = rdr["PostalCode"] != DBNull.Value ? rdr["PostalCode"].ToString() : null;
                    customer.Country = rdr["Country"] != DBNull.Value ? rdr["Country"].ToString() : null;
                    customer.Phone = rdr["Phone"] != DBNull.Value ? rdr["Phone"].ToString() : null;
                    customer.Fax = rdr["Fax"] != DBNull.Value ? rdr["Fax"].ToString() : null;
                    customers.Add(customer);
                }
            }
            return customers;
        }

        public List<Customer> SearchCustomers(string searchTerm)
        {
            var customers = new List<Customer>();
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM Customers WHERE CompanyName LIKE @Search OR ContactName LIKE @Search", cn);
                cmd.Parameters.AddWithValue("@Search", $"%{searchTerm}%");
                cn.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var customer = new Customer();
                    customer.CustomerId = rdr["CustomerID"].ToString();
                    customer.CompanyName = rdr["CompanyName"].ToString();
                    customers.Add(customer);
                }
            }
            return customers;
        }

        public (List<Customer> Items, int TotalCount) GetCustomersPaged(int page, int pageSize)
        {
            var customers = new List<Customer>();
            int totalCount = 0;

            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var countCmd = new SqlCommand("SELECT COUNT(*) FROM Customers", cn);
                cn.Open();
                totalCount = (int)countCmd.ExecuteScalar();

                var cmd = new SqlCommand(@"
                    SELECT * FROM Customers
                    ORDER BY CompanyName
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", cn);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var customer = new Customer();
                    customer.CustomerId = rdr["CustomerID"].ToString();
                    customer.CompanyName = rdr["CompanyName"].ToString();
                    customer.ContactName = rdr.IsDBNull("ContactName") ? null : rdr["ContactName"].ToString();
                    customer.ContactTitle = rdr.IsDBNull("ContactTitle") ? null : rdr["ContactTitle"].ToString();
                    customer.City = rdr.IsDBNull("City") ? null : rdr["City"].ToString();
                    customer.Country = rdr.IsDBNull("Country") ? null : rdr["Country"].ToString();
                    customer.Phone = rdr.IsDBNull("Phone") ? null : rdr["Phone"].ToString();
                    customers.Add(customer);
                }
            }
            return (customers, totalCount);
        }
    }

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

    public class EmployeeService
    {
        public bool AddEmployee(Employee employee)
        {
            if (!ValidateEmployee(employee))
                return false;

            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand("AddEmployee", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@Title", employee.Title ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", employee.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", employee.Region ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PostalCode", employee.PostalCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", employee.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HomePhone", employee.HomePhone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Extension", employee.Extension ?? (object)DBNull.Value);
                
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool UpdateEmployee(Employee employee)
        {
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand("UpdateEmployee", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeID", employee.EmployeeId);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@Title", employee.Title ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", employee.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", employee.Region ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PostalCode", employee.PostalCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", employee.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HomePhone", employee.HomePhone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Extension", employee.Extension ?? (object)DBNull.Value);
                
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool ValidateEmployee(Employee employee)
        {
            if (employee == null)
                return false;

            if (string.IsNullOrWhiteSpace(employee.LastName))
                return false;

            if (string.IsNullOrWhiteSpace(employee.FirstName))
                return false;

            if (employee.BirthDate.HasValue && employee.HireDate.HasValue)
            {
                if (employee.BirthDate.Value >= employee.HireDate.Value)
                    return false;
            }

            return true;
        }

        public DataTable GetEmployeeTerritories(int employeeId)
        {
            var dt = new DataTable();
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT t.TerritoryID, t.TerritoryDescription, t.RegionID
                    FROM EmployeeTerritories et
                    JOIN Territories t ON et.TerritoryID = t.TerritoryID
                    WHERE et.EmployeeID = @EmployeeID", cn);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
    }

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

    public class WorkflowService
    {
        public bool ProcessCompleteOrderWorkflow(Order order, List<OrderDetail> details, string paymentCardNumber, string paymentExpiry)
        {
            var orderService = new OrderService();
            var productService = new ProductService();

            foreach (var detail in details)
            {
                var product = productService.GetProductById(detail.ProductId);
                if (product == null || product.UnitsInStock < detail.Quantity)
                {
                    GlobalApplicationHelper.LogError(new Exception($"Insufficient stock for product {detail.ProductId}"), "Workflow");
                    return false;
                }
            }

            if (!GlobalApplicationHelper.ProcessPayment(order.CustomerId, 
                orderService.CalculateOrderTotal(order, details), paymentCardNumber, paymentExpiry))
            {
                return false;
            }

            if (!orderService.CreateOrder(order, details))
            {
                return false;
            }

            return true;
        }

        public bool ProcessOrderCancellationWorkflow(int orderId, string reason)
        {
            var orderService = new OrderService();
            var order = orderService.GetOrderById(orderId);
            
            if (order == null)
                return false;

            if (order.ShippedDate.HasValue)
            {
                GlobalApplicationHelper.LogError(new Exception($"Cannot cancel shipped order {orderId}"), "CancelWorkflow");
                return false;
            }

            return orderService.CancelOrder(orderId);
        }

        public bool ProcessReturnWorkflow(int orderId, int productId, int quantity)
        {
            var orderService = new OrderService();
            var productService = new ProductService();
            var details = orderService.GetOrderDetails(orderId);

            var detail = details.FirstOrDefault(d => d.ProductId == productId);
            if (detail == null)
                return false;

            if (quantity > detail.Quantity)
                quantity = detail.Quantity;

            productService.UpdateStock(productId, quantity);

            GlobalApplicationHelper.LogError(new Exception($"Return processed for order {orderId}, product {productId}, qty {quantity}"), "Return");

            return true;
        }
    }
}
