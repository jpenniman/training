using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public static class ConnectionHelper
    {
        public static SqlConnection GetConnection()
        {
            string cnStr = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            return new SqlConnection(cnStr);
        }

        public static string GetNullableString(this SqlDataReader rdr, string columnName)
        {
            return rdr.IsDBNull(columnName) ? null : rdr.GetString(columnName);
        }

        public static SqlParameter AddWithValue(this SqlParameterCollection collection, string name,
            SqlDbType sqlDbType, int size, object value)
        {
            return new SqlParameter(name, sqlDbType, size) { Value = value ?? DBNull.Value };
        }
    }
}
