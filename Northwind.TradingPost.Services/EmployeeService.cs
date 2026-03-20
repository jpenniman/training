using Northwind.TradingPost.Common;
using Northwind.TradingPost.Domain;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Services;

public class EmployeeService
{
    public bool AddEmployee(Employee employee)
    {
        if (!ValidateEmployee(employee))
            return false;

        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand("AddEmployee", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LastName", employee.LastName);
            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@Title", employee.Title ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HireDate", employee.HireDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@City", employee.City ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Region", employee.Region ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", employee.PostalCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", employee.Country ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HomePhone", employee.HomePhone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Extension", employee.Extension ?? (object)DBNull.Value);
                
            cn.Open();
            cmd.ExecuteNonQuery();
            return true;
        }
    }

    public bool UpdateEmployee(Employee employee)
    {
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand("UpdateEmployee", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", employee.EmployeeId);
            cmd.Parameters.AddWithValue("@LastName", employee.LastName);
            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@Title", employee.Title ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HireDate", employee.HireDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@City", employee.City ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Region", employee.Region ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", employee.PostalCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", employee.Country ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HomePhone", employee.HomePhone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Extension", employee.Extension ?? (object)DBNull.Value);
                
            cn.Open();
            cmd.ExecuteNonQuery();
            return true;
        }
    }

    public bool ValidateEmployee(Employee employee)
    {
        if (employee == null)
            return false;

        if (string.IsNullOrWhiteSpace(employee.LastName))
            return false;

        if (string.IsNullOrWhiteSpace(employee.FirstName))
            return false;

        if (employee.BirthDate.HasValue && employee.HireDate.HasValue)
        {
            if (employee.BirthDate.Value >= employee.HireDate.Value)
                return false;
        }

        return true;
    }

    public DataTable GetEmployeeTerritories(int employeeId)
    {
        var dt = new DataTable();
        using (var cn = GlobalApplicationHelper.GetDbConnection())
        {
            var cmd = new SqlCommand(@"
                    SELECT t.TerritoryID, t.TerritoryDescription, t.RegionID
                    FROM EmployeeTerritories et
                    JOIN Territories t ON et.TerritoryID = t.TerritoryID
                    WHERE et.EmployeeID = @EmployeeID", cn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
        }
        return dt;
    }
}