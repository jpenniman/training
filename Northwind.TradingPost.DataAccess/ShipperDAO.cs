using Northwind.TradingPost.Domain;
using System;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class ShipperDAO
    {
        public Shipper GetById(int id)
        {
            Shipper s = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("GetShipper", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShipperID", id);
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                s = new Shipper();
                s.ShipperId = Convert.ToInt32(rdr["ShipperID"]);
                s.CompanyName = Convert.ToString(rdr["CompanyName"]);
                s.Phone = Convert.ToString(rdr["Phone"]);
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return s;
        }

        public bool Add(Shipper shipper)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddShipper", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyName", shipper.CompanyName);
            cmd.Parameters.AddWithValue("@Phone", shipper.Phone);
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(Shipper shipper)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateShipper", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShipperID", shipper.ShipperId);
            cmd.Parameters.AddWithValue("@CompanyName", shipper.CompanyName);
            cmd.Parameters.AddWithValue("@Phone", shipper.Phone);
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
            SqlCommand cmd = new SqlCommand("DeleteShipper", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShipperID", id);
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }
    }
}
