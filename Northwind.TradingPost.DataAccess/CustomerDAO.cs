using Northwind.TradingPost.Domain;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class CustomerDAO
    {
        public Customer GetById(string id)
        {
            Customer c = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("GetCustomer", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.NChar, 5){ Value = id });
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                c = new Customer();
                var customerIdOrdinal = rdr.GetOrdinal("CustomerID");
                c.CustomerId = rdr.GetString(customerIdOrdinal); 
                c.CompanyName = rdr.GetString("CompanyName"); //Extension method as of net471 and netCoreApp1.0.
            
                // This will return an empty string if the value is DbNull
                c.ContactName = rdr.GetString("ContactName");

                // This is one way to check for null.
                if (!rdr.IsDBNull("ContactTitle"))
                    c.ContactTitle = rdr.GetString("ContactTitle");

                // Ternary operator
                c.Address = rdr.IsDBNull("Address") ? null : rdr.GetString("Address");
                c.City = rdr.IsDBNull("City") ? null : rdr.GetString("City");

                // Custom extension method
                c.Region = rdr.GetNullableString("Region");
                c.PostalCode = rdr.GetNullableString("PostalCode");
                c.Country = rdr.GetNullableString("Country");
                c.Phone = rdr.GetNullableString("Phone");
                c.Fax = rdr.GetNullableString("Fax");
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return c;
        }

        public bool Add(Customer customer)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddCustomer", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@CustomerID", SqlDbType.NChar, 5).Direction = ParameterDirection.Output;
            cmd.Parameters.Add(new SqlParameter("@CompanyName", SqlDbType.NVarChar, 40){ Value = customer.CompanyName });
            cmd.Parameters.Add(new SqlParameter("@ContactName", SqlDbType.NVarChar, 30) { Value = customer.ContactName });
        
            //Custom extension method
            cmd.Parameters.AddWithValue("@ContactTitle", SqlDbType.NVarChar, 40, customer.ContactTitle);
            cmd.Parameters.AddWithValue("@Address", customer.Address);
            cmd.Parameters.AddWithValue("@City", customer.City);
            cmd.Parameters.AddWithValue("@Region", customer.Region);
            cmd.Parameters.AddWithValue("@PostalCode", customer.PostalCode);
            cmd.Parameters.AddWithValue("@Country", customer.Country);
            cmd.Parameters.AddWithValue("@Phone", customer.Phone);
            cmd.Parameters.AddWithValue("@Fax", customer.Fax);
            cn.Open();
            cmd.ExecuteNonQuery();
            customer.CustomerId = cmd.Parameters["@CustomerId"].Value.ToString();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(Customer customer)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateCustomer", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
            cmd.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
            cmd.Parameters.AddWithValue("@ContactName", customer.ContactName);
            cmd.Parameters.AddWithValue("@ContactTitle", customer.ContactTitle);
            cmd.Parameters.AddWithValue("@Address", customer.Address);
            cmd.Parameters.AddWithValue("@City", customer.City);
            cmd.Parameters.AddWithValue("@Region", customer.Region);
            cmd.Parameters.AddWithValue("@PostalCode", customer.PostalCode);
            cmd.Parameters.AddWithValue("@Country", customer.Country);
            cmd.Parameters.AddWithValue("@Phone", customer.Phone);
            cmd.Parameters.AddWithValue("@Fax", customer.Fax);
            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Delete(string id)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("DeleteCustomer", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", id);
            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
            cn.Dispose();
            return true;
        }

        SqlParameter CreateIdParameter(string customerId)
        {
            return new SqlParameter("@CustomerID", SqlDbType.NChar, 5) { Value = (object) customerId ?? DBNull.Value };
        }
    }
}
