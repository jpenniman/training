using Northwind.TradingPost.Domain;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class ProductDAO
    {
        public Product GetById(int id)
        {
            Product p = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            // There is no GetProduct stored procedure, so using a direct SQL query
            SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID = @ProductID", cn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@ProductID", id);
            
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                p = new Product();
                p.ProductId = Convert.ToInt32(rdr["ProductID"]);
                p.ProductName = Convert.ToString(rdr["ProductName"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("SupplierID")))
                    p.SupplierId = Convert.ToInt32(rdr["SupplierID"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("CategoryID")))
                    p.CategoryId = Convert.ToInt32(rdr["CategoryID"]);
                
                p.QuantityPerUnit = Convert.ToString(rdr["QuantityPerUnit"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("UnitPrice")))
                    p.UnitPrice = Convert.ToDecimal(rdr["UnitPrice"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("UnitsInStock")))
                    p.UnitsInStock = Convert.ToInt16(rdr["UnitsInStock"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("UnitsOnOrder")))
                    p.UnitsOnOrder = Convert.ToInt16(rdr["UnitsOnOrder"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("ReorderLevel")))
                    p.ReorderLevel = Convert.ToInt16(rdr["ReorderLevel"]);
                
                p.Discontinued = Convert.ToBoolean(rdr["Discontinued"]);
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return p;
        }

        public bool Add(Product product)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddProduct", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ProductID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
            cmd.Parameters.AddWithValue("@SupplierID", (object)product.SupplierId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoryID", (object)product.CategoryId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@QuantityPerUnit", (object)product.QuantityPerUnit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnitPrice", (object)product.UnitPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnitsInStock", (object)product.UnitsInStock ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnitsOnOrder", (object)product.UnitsOnOrder ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReorderLevel", (object)product.ReorderLevel ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Discontinued", product.Discontinued);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            product.ProductId = Convert.ToInt32(cmd.Parameters["@ProductID"].Value);
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(Product product)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateProduct", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", product.ProductId);
            cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
            cmd.Parameters.AddWithValue("@SupplierID", (object)product.SupplierId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoryID", (object)product.CategoryId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@QuantityPerUnit", (object)product.QuantityPerUnit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnitPrice", (object)product.UnitPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnitsInStock", (object)product.UnitsInStock ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnitsOnOrder", (object)product.UnitsOnOrder ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReorderLevel", (object)product.ReorderLevel ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Discontinued", product.Discontinued);
            
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
            SqlCommand cmd = new SqlCommand("DeleteProduct", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", id);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }
    }
}