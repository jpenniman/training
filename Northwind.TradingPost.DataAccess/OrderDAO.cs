using Northwind.TradingPost.Domain;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class OrderDAO
    {
        public Order GetById(int id)
        {
            Order o = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            // There is no GetOrder stored procedure, so using a direct SQL query
            SqlCommand cmd = new SqlCommand("SELECT * FROM Orders WHERE OrderID = @OrderID", cn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@OrderID", id);
            
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                o = new Order();
                o.OrderId = Convert.ToInt32(rdr["OrderID"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("CustomerID")))
                    o.CustomerId = Convert.ToString(rdr["CustomerID"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("EmployeeID")))
                    o.EmployeeId = Convert.ToInt32(rdr["EmployeeID"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("OrderDate")))
                    o.OrderDate = Convert.ToDateTime(rdr["OrderDate"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")))
                    o.RequiredDate = Convert.ToDateTime(rdr["RequiredDate"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")))
                    o.ShippedDate = Convert.ToDateTime(rdr["ShippedDate"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("ShipVia")))
                    o.ShipVia = Convert.ToInt32(rdr["ShipVia"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("Freight")))
                    o.Freight = Convert.ToDecimal(rdr["Freight"]);
                
                o.ShipName = Convert.ToString(rdr["ShipName"]);
                o.ShipAddress = Convert.ToString(rdr["ShipAddress"]);
                o.ShipCity = Convert.ToString(rdr["ShipCity"]);
                o.ShipRegion = Convert.ToString(rdr["ShipRegion"]);
                o.ShipPostalCode = Convert.ToString(rdr["ShipPostalCode"]);
                o.ShipCountry = Convert.ToString(rdr["ShipCountry"]);
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return o;
        }

        public bool Add(Order order)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddOrder", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@CustomerID", (object)order.CustomerId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeID", (object)order.EmployeeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OrderDate", (object)order.OrderDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RequiredDate", (object)order.RequiredDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShippedDate", (object)order.ShippedDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipVia", (object)order.ShipVia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Freight", (object)order.Freight ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipName", (object)order.ShipName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipAddress", (object)order.ShipAddress ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipCity", (object)order.ShipCity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipRegion", (object)order.ShipRegion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipPostalCode", (object)order.ShipPostalCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipCountry", (object)order.ShipCountry ?? DBNull.Value);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            order.OrderId = Convert.ToInt32(cmd.Parameters["@OrderID"].Value);
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(Order order)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateOrder", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", order.OrderId);
            cmd.Parameters.AddWithValue("@CustomerID", (object)order.CustomerId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeID", (object)order.EmployeeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OrderDate", (object)order.OrderDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RequiredDate", (object)order.RequiredDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShippedDate", (object)order.ShippedDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipVia", (object)order.ShipVia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Freight", (object)order.Freight ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipName", (object)order.ShipName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipAddress", (object)order.ShipAddress ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipCity", (object)order.ShipCity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipRegion", (object)order.ShipRegion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipPostalCode", (object)order.ShipPostalCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipCountry", (object)order.ShipCountry ?? DBNull.Value);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Delete(int id)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("DeleteOrder", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", id);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }
    }
}