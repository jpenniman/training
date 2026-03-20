using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Web;

namespace Northwind.TradingPost.Common;

public static class GlobalApplicationHelper
{
    private static string _connectionString;
    private static string _smtpServer;
    private static int _smtpPort;
    private static string _emailFrom;
    private static string _adminEmail;

    static GlobalApplicationHelper()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["Default"]?.ConnectionString 
                            ?? "Server=localhost;Database=Northwind;Trusted_Connection=True;";
        _smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "localhost";
        _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "25");
        _emailFrom = ConfigurationManager.AppSettings["EmailFrom"] ?? "noreply@northwind.com";
        _adminEmail = ConfigurationManager.AppSettings["AdminEmail"] ?? "admin@northwind.com";
    }

    public static SqlConnection GetDbConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public static bool IsValidCustomerId(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return false;
        if (customerId.Length != 5)
            return false;
        return true;
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static string FormatCustomerName(string firstName, string lastName)
    {
        return $"{lastName}, {firstName}";
    }

    public static decimal CalculateOrderTotal(DataTable orderDetails)
    {
        decimal total = 0;
        foreach (DataRow row in orderDetails.Rows)
        {
            decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);
            int quantity = Convert.ToInt32(row["Quantity"]);
            decimal discount = Convert.ToDecimal(row["Discount"]);
            total += (unitPrice * quantity) * (1 - discount);
        }
        return total;
    }

    public static void SendEmail(string to, string subject, string body)
    {
        // using (var client = new SmtpClient(_smtpServer, _smtpPort))
        // {
        //     var message = new MailMessage(_emailFrom, to, subject, body);
        //     client.Send(message);
        // }
    }

    public static void LogError(Exception ex, string context = "")
    {
        string logPath = ConfigurationManager.AppSettings["ErrorLogPath"] ?? "C:\\Logs";
        Directory.CreateDirectory(logPath);
        string fileName = Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd}.log");
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {ex.Message}\n{ex.StackTrace}\n\n";
        File.AppendAllText(fileName, logEntry);
    }

    public static DataTable GetOrdersByCustomer(string customerId)
    {
        var dt = new DataTable();
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT * FROM Orders WHERE CustomerID = @CustomerID", cn);
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public static DataTable GetOrderDetails(int orderId)
    {
        var dt = new DataTable();
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT * FROM [Order Details] WHERE OrderID = @OrderID", cn);
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public static bool UpdateProductStock(int productId, int quantityChange)
    {
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("UPDATE Products SET UnitsInStock = UnitsInStock + @Change WHERE ProductID = @ProductID", cn);
            cmd.Parameters.AddWithValue("@ProductID", productId);
            cmd.Parameters.AddWithValue("@Change", quantityChange);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public static string GenerateOrderConfirmationNumber(int orderId)
    {
        return $"ORD-{orderId}-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
    }

    public static bool ValidateOrderCanShip(DateTime? orderDate, DateTime? requiredDate)
    {
        if (!orderDate.HasValue || !requiredDate.HasValue)
            return false;
        var daysToShip = (requiredDate.Value - orderDate.Value).Days;
        return daysToShip >= 1;
    }

    public static decimal CalculateShippingCost(decimal orderTotal, string shipCountry)
    {
        if (orderTotal > 1000)
            return 0;
        if (shipCountry == "USA")
            return orderTotal * 0.05m;
        return orderTotal * 0.15m;
    }

    public static DataTable GetCustomerOrderHistory(string customerId)
    {
        var dt = new DataTable();
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("CustOrderHist", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public static List<string> GetTop10ExpensiveProducts()
    {
        var products = new List<string>();
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("\"Ten Most Expensive Products\"", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                products.Add(rdr["TenMostExpensiveProducts"].ToString());
            }
        }
        return products;
    }

    public static bool ApplyDiscount(string customerId, decimal discountPercent)
    {
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("UPDATE Customers SET Discount = @Discount WHERE CustomerID = @CustomerID", cn);
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cmd.Parameters.AddWithValue("@Discount", discountPercent);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public static string GetCustomerDiscount(string customerId)
    {
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT Discount FROM Customers WHERE CustomerID = @CustomerID", cn);
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cn.Open();
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "0";
        }
    }

    public static DataTable GetSalesByYear(DateTime? startDate, DateTime? endDate)
    {
        var dt = new DataTable();
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("Sales by Year", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Beginning_Date", startDate ?? DateTime.Now.AddYears(-1));
            cmd.Parameters.AddWithValue("@Ending_Date", endDate ?? DateTime.Now);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public static DataTable GetSalesByCategory(string categoryName, string ordYear)
    {
        var dt = new DataTable();
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("SalesByCategory", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CategoryName", categoryName);
            cmd.Parameters.AddWithValue("@OrdYear", ordYear);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }

    public static bool DeleteCustomer(string customerId)
    {
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("DeleteCustomer", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cn.Open();
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public static string GetNextEmployeeId()
    {
        using (var cn = GetDbConnection())
        {
            var cmd = new SqlCommand("SELECT MAX(EmployeeID) + 1 FROM Employees", cn);
            cn.Open();
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "1";
        }
    }

    public static bool ProcessPayment(string customerId, decimal amount, string cardNumber, string cardExp)
    {
        LogError(new Exception($"Payment processed: {customerId}, ${amount}, Card: {cardNumber.Substring(0, 4)}****"), "Payment");
        return true;
    }

    public static string HtmlEncode(string input)
    {
        return HttpUtility.HtmlEncode(input);
    }

    public static string GetAppVersion()
    {
        return "1.0.0.0";
    }

    public static bool IsProduction()
    {
        var env = ConfigurationManager.AppSettings["Environment"];
        return env == "Production";
    }
}