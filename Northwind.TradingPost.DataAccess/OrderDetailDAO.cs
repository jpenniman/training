using Northwind.TradingPost.Domain;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class OrderDetailDAO
    {
        public OrderDetail GetByIds(int orderId, int productId)
        {
            OrderDetail od = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            // There is no GetOrderDetail stored procedure, so using a direct SQL query
            SqlCommand cmd = new SqlCommand("SELECT * FROM [Order Details] WHERE OrderID = @OrderID AND ProductID = @ProductID", cn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            cmd.Parameters.AddWithValue("@ProductID", productId);
            
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                od = new OrderDetail();
                od.OrderId = Convert.ToInt32(rdr["OrderID"]);
                od.ProductId = Convert.ToInt32(rdr["ProductID"]);
                od.UnitPrice = Convert.ToDecimal(rdr["UnitPrice"]);
                od.Quantity = Convert.ToInt16(rdr["Quantity"]);
                od.Discount = Convert.ToSingle(rdr["Discount"]);
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return od;
        }

        public bool Add(OrderDetail orderDetail)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddOrderDetail", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", orderDetail.OrderId);
            cmd.Parameters.AddWithValue("@ProductID", orderDetail.ProductId);
            cmd.Parameters.AddWithValue("@UnitPrice", orderDetail.UnitPrice);
            cmd.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
            cmd.Parameters.AddWithValue("@Discount", orderDetail.Discount);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(OrderDetail orderDetail)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateOrderDetail", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", orderDetail.OrderId);
            cmd.Parameters.AddWithValue("@ProductID", orderDetail.ProductId);
            cmd.Parameters.AddWithValue("@UnitPrice", orderDetail.UnitPrice);
            cmd.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
            cmd.Parameters.AddWithValue("@Discount", orderDetail.Discount);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Delete(int orderId, int productId)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("DeleteOrderDetail", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            cmd.Parameters.AddWithValue("@ProductID", productId);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }
    }
}