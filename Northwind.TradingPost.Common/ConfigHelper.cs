using System.Configuration;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Common;

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