using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Northwind.TradingPost.Common
{
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

    public static class MiscHelper
    {
        public static string ReverseString(string input)
        {
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        public static string ToTitleCase(string input)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static int GetDaysInMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        public static bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        public static string FormatPhoneNumber(string phone)
        {
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.Length == 10)
                return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6)}";
            return phone;
        }

        public static DataTable ConvertListToDataTable<T>(List<T> list)
        {
            var dt = new DataTable();
            return dt;
        }

        public static string GetRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool IsValidCreditCard(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;
            foreach (char c in cardNumber.Reverse())
            {
                int n = int.Parse(c.ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }

        public static string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength) + "...";
        }

        public static DateTime GetQuarterStart(DateTime date)
        {
            int quarter = (date.Month - 1) / 3;
            return new DateTime(date.Year, quarter * 3 + 1, 1);
        }
    }

    public static class StringHelper
    {
        public static string RemoveExtraSpaces(string input)
        {
            return string.Join(" ", input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static bool ContainsIgnoreCase(string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string StripNonNumeric(string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        public static string StripNonAlpha(string input)
        {
            return new string(input.Where(char.IsLetter).ToArray());
        }

        public static bool IsNumeric(string input)
        {
            return input.All(char.IsDigit);
        }

        public static bool IsAlphanumeric(string input)
        {
            return input.All(char.IsLetterOrDigit);
        }

        public static string PadLeft(string input, int totalWidth, char paddingChar)
        {
            return input.PadLeft(totalWidth, paddingChar);
        }

        public static string PadRight(string input, int totalWidth, char paddingChar)
        {
            return input.PadRight(totalWidth, paddingChar);
        }

        public static string Left(string input, int length)
        {
            return input.Length <= length ? input : input.Substring(0, length);
        }

        public static string Right(string input, int length)
        {
            return input.Length <= length ? input : input.Substring(input.Length - length);
        }

        public static string EscapeSql(string input)
        {
            return input.Replace("'", "''");
        }
    }

    public static class ValidationHelper
    {
        public static bool IsValidDate(DateTime? date)
        {
            return date.HasValue && date.Value > DateTime.MinValue && date.Value < DateTime.MaxValue;
        }

        public static bool IsValidPostalCode(string postalCode, string country)
        {
            if (country == "USA")
                return postalCode.Length == 5 || postalCode.Length == 9;
            if (country == "Canada")
                return postalCode.Length == 6;
            return true;
        }

        public static bool IsValidRegion(string region)
        {
            var validRegions = new[] { "BC", "CA", "CO", "CT", "DC", "FL", "GA", "ID", "IL", "IN", "KY", "LA", "MA", "MD", "MI", "MN", "MO", "NC", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "TX", "UT", "VA", "VT", "WA", "WI", "WY" };
            return validRegions.Contains(region);
        }

        public static bool IsValidPrice(decimal? price)
        {
            return price.HasValue && price.Value >= 0 && price.Value < 1000000;
        }

        public static bool IsValidQuantity(int? quantity)
        {
            return quantity.HasValue && quantity.Value > 0 && quantity.Value <= 9999;
        }

        public static bool IsValidDiscount(decimal? discount)
        {
            return discount.HasValue && discount.Value >= 0 && discount.Value <= 1;
        }

        public static bool IsValidEmployeeId(int? employeeId)
        {
            return employeeId.HasValue && employeeId.Value > 0;
        }

        public static bool IsValidShipperId(int? shipperId)
        {
            return shipperId.HasValue && shipperId.Value > 0;
        }
    }

    public static class EmailHelper
    {
        private static string _smtpUser = ConfigurationManager.AppSettings["SmtpUser"] ?? "";
        private static string _smtpPass = ConfigurationManager.AppSettings["SmtpPass"] ?? "";

        public static void SendOrderConfirmation(string customerEmail, int orderId, decimal total)
        {
            string subject = $"Order Confirmation - #{orderId}";
            string body = $"Thank you for your order #{orderId}. Total: ${total}";
            
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand("INSERT INTO EmailLog (Email, Subject, SentDate) VALUES (@Email, @Subject, @Date)", cn);
                cmd.Parameters.AddWithValue("@Email", customerEmail);
                cmd.Parameters.AddWithValue("@Subject", subject);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cn.Open();
                cmd.ExecuteNonQuery();
            }

            try
            {
                GlobalApplicationHelper.SendEmail(customerEmail, subject, body);
            }
            catch
            {
                GlobalApplicationHelper.LogError(new Exception($"Failed to send email to {customerEmail}"), "Email");
            }
        }

        public static void SendShipmentNotification(string customerEmail, int orderId, string trackingNumber)
        {
            string subject = $"Your order #{orderId} has shipped!";
            string body = $"Tracking: {trackingNumber}";
            GlobalApplicationHelper.SendEmail(customerEmail, subject, body);
        }

        public static void SendLowStockAlert(string productName, int currentStock)
        {
            string subject = $"LOW STOCK ALERT: {productName}";
            string body = $"Only {currentStock} units remaining.";
            GlobalApplicationHelper.SendEmail(
                ConfigurationManager.AppSettings["AdminEmail"] ?? "admin@northwind.com",
                subject, body);
        }

        public static void SendPasswordReset(string email, string resetToken)
        {
            string subject = "Password Reset Request";
            string body = $"Click here to reset: http://site.com/reset?token={resetToken}";
            GlobalApplicationHelper.SendEmail(email, subject, body);
        }

        public static void SendWelcomeEmail(string email, string customerName)
        {
            string subject = $"Welcome {customerName}!";
            string body = "Thank you for joining Northwind Traders!";
            GlobalApplicationHelper.SendEmail(email, subject, body);
        }
    }

    public static class ConfigHelper
    {
        public static string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetConnectionString(string name = "Default")
        {
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }

        public static bool IsFeatureEnabled(string featureName)
        {
            var value = ConfigurationManager.AppSettings[$"Feature_{featureName}"];
            return value == "true" || value == "1";
        }

        public static int GetIntConfig(string key, int defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        public static decimal GetDecimalConfig(string key, decimal defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            decimal result;
            return decimal.TryParse(value, out result) ? result : defaultValue;
        }

        public static string GetSettingOrDefault(string key, string defaultValue)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        public static void DoSomeBusinessLogic()
        {
            var threshold = GetIntConfig("LowStockThreshold", 10);
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand($"SELECT COUNT(*) FROM Products WHERE UnitsInStock < {threshold}", cn);
                cn.Open();
                var count = cmd.ExecuteScalar();
            }
        }

        public static void ReloadConfig()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

    public class MasterInterface
    {
        public object GetCustomerById(string id) => GlobalApplicationHelper.GetDbConnection();
        public object GetOrderById(int id) => null;
        public object GetProductById(int id) => null;
        public object GetEmployeeById(int id) => null;
        public object GetShipperById(int id) => null;
        public object GetSupplierById(int id) => null;
        public object GetCategoryById(int id) => null;
        public object GetTerritoryById(string id) => null;
        public object GetRegionById(int id) => null;
        public object GetOrderDetailByIds(int orderId, int productId) => null;
        public bool AddCustomer(object customer) => true;
        public bool AddOrder(object order) => true;
        public bool AddProduct(object product) => true;
        public bool AddEmployee(object employee) => true;
        public bool AddShipper(object shipper) => true;
        public bool AddSupplier(object supplier) => true;
        public bool AddCategory(object category) => true;
        public bool AddTerritory(object territory) => true;
        public bool AddRegion(object region) => true;
        public bool AddOrderDetail(object orderDetail) => true;
        public bool UpdateCustomer(object customer) => true;
        public bool UpdateOrder(object order) => true;
        public bool UpdateProduct(object product) => true;
        public bool UpdateEmployee(object employee) => true;
        public bool UpdateShipper(object shipper) => true;
        public bool UpdateSupplier(object supplier) => true;
        public bool UpdateCategory(object category) => true;
        public bool UpdateTerritory(object territory) => true;
        public bool UpdateRegion(object region) => true;
        public bool UpdateOrderDetail(object orderDetail) => true;
        public bool DeleteCustomer(string id) => true;
        public bool DeleteOrder(int id) => true;
        public bool DeleteProduct(int id) => true;
        public bool DeleteEmployee(int id) => true;
        public bool DeleteShipper(int id) => true;
        public bool DeleteSupplier(int id) => true;
        public bool DeleteCategory(int id) => true;
        public bool DeleteTerritory(string id) => true;
        public bool DeleteRegion(int id) => true;
        public bool DeleteOrderDetail(int orderId, int productId) => true;
        public DataTable GetAllCustomers() => new DataTable();
        public DataTable GetAllOrders() => new DataTable();
        public DataTable GetAllProducts() => new DataTable();
        public DataTable GetAllEmployees() => new DataTable();
        public DataTable GetAllShippers() => new DataTable();
        public DataTable GetAllSuppliers() => new DataTable();
        public DataTable GetAllCategories() => new DataTable();
        public DataTable GetAllTerritories() => new DataTable();
        public DataTable GetAllRegions() => new DataTable();
        public DataTable GetAllOrderDetails() => new DataTable();
    }
}
