using Northwind.TradingPost.Domain;
using System;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class SupplierDAO
    {
        public Supplier GetById(int id)
        {
            Supplier s = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("GetSupplier", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierID", id);
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                s = new Supplier();
                s.SupplierId = Convert.ToInt32(rdr["SupplierID"]);
                s.CompanyName = Convert.ToString(rdr["CompanyName"]);
                s.ContactName = Convert.ToString(rdr["ContactName"]);
                s.ContactTitle = Convert.ToString(rdr["ContactTitle"]);
                s.Address = Convert.ToString(rdr["Address"]);
                s.City = Convert.ToString(rdr["City"]);
                s.Region = Convert.ToString(rdr["Region"]);
                s.PostalCode = Convert.ToString(rdr["PostalCode"]);
                s.Country = Convert.ToString(rdr["Country"]);
                s.Phone = Convert.ToString(rdr["Phone"]);
                s.Fax = Convert.ToString(rdr["Fax"]);
                s.HomePage = Convert.ToString(rdr["HomePage"]);
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return s;
        }

        public bool Add(Supplier supplier)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddSupplier", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierId);
            cmd.Parameters.AddWithValue("@CompanyName", supplier.CompanyName);
            cmd.Parameters.AddWithValue("@ContactName", supplier.ContactName);
            cmd.Parameters.AddWithValue("@ContactTitle", supplier.ContactTitle);
            cmd.Parameters.AddWithValue("@Address", supplier.Address);
            cmd.Parameters.AddWithValue("@City", supplier.City);
            cmd.Parameters.AddWithValue("@Region", supplier.Region);
            cmd.Parameters.AddWithValue("@PostalCode", supplier.PostalCode);
            cmd.Parameters.AddWithValue("@Country", supplier.Country);
            cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
            cmd.Parameters.AddWithValue("@Fax", supplier.Fax);
            cmd.Parameters.AddWithValue("@HomePage", supplier.HomePage);
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(Supplier supplier)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateSupplier", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierId);
            cmd.Parameters.AddWithValue("@CompanyName", supplier.CompanyName);
            cmd.Parameters.AddWithValue("@ContactName", supplier.ContactName);
            cmd.Parameters.AddWithValue("@ContactTitle", supplier.ContactTitle);
            cmd.Parameters.AddWithValue("@Address", supplier.Address);
            cmd.Parameters.AddWithValue("@City", supplier.City);
            cmd.Parameters.AddWithValue("@Region", supplier.Region);
            cmd.Parameters.AddWithValue("@PostalCode", supplier.PostalCode);
            cmd.Parameters.AddWithValue("@Country", supplier.Country);
            cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
            cmd.Parameters.AddWithValue("@Fax", supplier.Fax);
            cmd.Parameters.AddWithValue("@HomePage", supplier.HomePage);
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
            SqlCommand cmd = new SqlCommand("DeleteSupplier", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierID", id);
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }
    }
}