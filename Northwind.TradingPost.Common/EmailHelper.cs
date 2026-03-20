using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Common;

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