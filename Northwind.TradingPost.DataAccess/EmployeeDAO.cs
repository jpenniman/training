using Northwind.TradingPost.Domain;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.DataAccess
{
    public class EmployeeDAO
    {
        public Employee GetById(int id)
        {
            Employee e = null;
            SqlConnection cn = ConnectionHelper.GetConnection();
            // There is no GetEmployee stored procedure, so using a direct SQL query
            SqlCommand cmd = new SqlCommand("SELECT * FROM Employees WHERE EmployeeID = @EmployeeID", cn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@EmployeeID", id);
            
            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                e = new Employee();
                e.EmployeeId = Convert.ToInt32(rdr["EmployeeID"]);
                e.LastName = Convert.ToString(rdr["LastName"]);
                e.FirstName = Convert.ToString(rdr["FirstName"]);
                e.Title = Convert.ToString(rdr["Title"]);
                e.TitleOfCourtesy = Convert.ToString(rdr["TitleOfCourtesy"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("BirthDate")))
                    e.BirthDate = Convert.ToDateTime(rdr["BirthDate"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("HireDate")))
                    e.HireDate = Convert.ToDateTime(rdr["HireDate"]);
                
                e.Address = Convert.ToString(rdr["Address"]);
                e.City = Convert.ToString(rdr["City"]);
                e.Region = Convert.ToString(rdr["Region"]);
                e.PostalCode = Convert.ToString(rdr["PostalCode"]);
                e.Country = Convert.ToString(rdr["Country"]);
                e.HomePhone = Convert.ToString(rdr["HomePhone"]);
                e.Extension = Convert.ToString(rdr["Extension"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("Photo")))
                    e.Photo = (byte[])rdr["Photo"];
                
                e.Notes = Convert.ToString(rdr["Notes"]);
                
                if (!rdr.IsDBNull(rdr.GetOrdinal("ReportsTo")))
                    e.ReportsTo = Convert.ToInt32(rdr["ReportsTo"]);
                
                e.PhotoPath = Convert.ToString(rdr["PhotoPath"]);
            }
            rdr.Close();
            rdr.Dispose();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return e;
        }

        public bool Add(Employee employee)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("AddEmployee", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@LastName", employee.LastName);
            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@Title", (object)employee.Title ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TitleOfCourtesy", (object)employee.TitleOfCourtesy ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BirthDate", (object)employee.BirthDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HireDate", (object)employee.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)employee.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object)employee.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Region", (object)employee.Region ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", (object)employee.PostalCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", (object)employee.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HomePhone", (object)employee.HomePhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Extension", (object)employee.Extension ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Photo", (object)employee.Photo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object)employee.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportsTo", (object)employee.ReportsTo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhotoPath", (object)employee.PhotoPath ?? DBNull.Value);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            employee.EmployeeId = Convert.ToInt32(cmd.Parameters["@EmployeeID"].Value);
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }

        public bool Update(Employee employee)
        {
            SqlConnection cn = ConnectionHelper.GetConnection();
            SqlCommand cmd = new SqlCommand("UpdateEmployee", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", employee.EmployeeId);
            cmd.Parameters.AddWithValue("@LastName", employee.LastName);
            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@Title", (object)employee.Title ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TitleOfCourtesy", (object)employee.TitleOfCourtesy ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BirthDate", (object)employee.BirthDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HireDate", (object)employee.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)employee.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object)employee.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Region", (object)employee.Region ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", (object)employee.PostalCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", (object)employee.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HomePhone", (object)employee.HomePhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Extension", (object)employee.Extension ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Photo", (object)employee.Photo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object)employee.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportsTo", (object)employee.ReportsTo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhotoPath", (object)employee.PhotoPath ?? DBNull.Value);
            
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
            SqlCommand cmd = new SqlCommand("DeleteEmployee", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", id);
            
            cn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
            cn.Dispose();
            return true;
        }
    }
}