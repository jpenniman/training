using Northwind.TradingPost.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class CategoryDAO
    {
        public Category GetById(int id)
        {
            Category c = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            // There is no GetCategory stored procedure, so using a direct SQL query
            SqlCommand cmd = new SqlCommand("SELECT * FROM Categories WHERE CategoryID = @CategoryID", cn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@CategoryID", id);
            
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                c = new Category();
                c.CategoryId = Convert.ToInt32(rdr["CategoryID"]);
                c.CategoryName = Convert.ToString(rdr["CategoryName"]);
                c.Description = Convert.ToString(rdr["Description"]);
                
                // Handle the picture as byte array
                if (!rdr.IsDBNull(rdr.GetOrdinal("Picture")))
                {
                    c.Picture = (byte[])rdr["Picture"];
                }
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return c;
        }

        public bool Add(Category category)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddCategory", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
            cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Picture", (object)category.Picture ?? DBNull.Value);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            category.CategoryId = Convert.ToInt32(cmd.Parameters["@CategoryID"].Value);
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(Category category)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateCategory", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CategoryID", category.CategoryId);
            cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
            cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Picture", (object)category.Picture ?? DBNull.Value);
            
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
            SqlCommand cmd = new SqlCommand("DeleteCategory", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CategoryID", id);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public List<Category> GetAll()
        {
            var categories = new List<Category>();
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName", cn);
            cmd.CommandType = CommandType.Text;
            
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var c = new Category();
                c.CategoryId = Convert.ToInt32(rdr["CategoryID"]);
                c.CategoryName = Convert.ToString(rdr["CategoryName"]);
                categories.Add(c);
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return categories;
        }
    }
}