using Northwind.TradingPost.Domain;
using Northwind.TradingPost.DataAccess;
using Northwind.TradingPost.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Northwind.TradingPost.Services
{
    public class CustomerService
    {
        private CustomerDAO _customerDao;

        public CustomerService()
        {
            _customerDao = new CustomerDAO();
        }

        public bool AddCustomer(Customer customer)
        {
            if (!ValidateCustomer(customer))
                return false;

            bool result = _customerDao.Add(customer);

            if (result && !string.IsNullOrEmpty(customer.Email))
            {
                EmailHelper.SendWelcomeEmail(customer.Email, customer.CompanyName);
            }

            return result;
        }

        public bool UpdateCustomer(Customer customer)
        {
            var existing = _customerDao.GetById(customer.CustomerId);
            if (existing == null)
                return false;

            return _customerDao.Update(customer);
        }

        public bool DeleteCustomer(string customerId)
        {
            var orders = GlobalApplicationHelper.GetOrdersByCustomer(customerId);
            if (orders.Rows.Count > 0)
            {
                GlobalApplicationHelper.LogError(new Exception($"Cannot delete customer {customerId} with existing orders"), "DeleteCustomer");
                return false;
            }

            return GlobalApplicationHelper.DeleteCustomer(customerId);
        }

        public bool ValidateCustomer(Customer customer)
        {
            if (customer == null)
                return false;

            if (string.IsNullOrWhiteSpace(customer.CustomerId))
                return false;

            if (customer.CustomerId.Length != 5)
                return false;

            if (!string.IsNullOrEmpty(customer.Email))
            {
                if (!GlobalApplicationHelper.IsValidEmail(customer.Email))
                    return false;
            }

            return true;
        }

        public bool ApplyDiscount(string customerId, decimal discountPercent)
        {
            return GlobalApplicationHelper.ApplyDiscount(customerId, discountPercent);
        }

        public string GetCustomerDiscount(string customerId)
        {
            return GlobalApplicationHelper.GetCustomerDiscount(customerId);
        }

        public List<Customer> GetCustomersByCountry(string country)
        {
            var customers = new List<Customer>();
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM Customers WHERE Country = @Country", cn);
                cmd.Parameters.AddWithValue("@Country", country);
                cn.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var customer = new Customer();
                    customer.CustomerId = rdr["CustomerID"].ToString();
                    customer.CompanyName = rdr["CompanyName"].ToString();
                    customer.ContactName = rdr["ContactName"] != DBNull.Value ? rdr["ContactName"].ToString() : null;
                    customer.ContactTitle = rdr["ContactTitle"] != DBNull.Value ? rdr["ContactTitle"].ToString() : null;
                    customer.Address = rdr["Address"] != DBNull.Value ? rdr["Address"].ToString() : null;
                    customer.City = rdr["City"] != DBNull.Value ? rdr["City"].ToString() : null;
                    customer.Region = rdr["Region"] != DBNull.Value ? rdr["Region"].ToString() : null;
                    customer.PostalCode = rdr["PostalCode"] != DBNull.Value ? rdr["PostalCode"].ToString() : null;
                    customer.Country = rdr["Country"] != DBNull.Value ? rdr["Country"].ToString() : null;
                    customer.Phone = rdr["Phone"] != DBNull.Value ? rdr["Phone"].ToString() : null;
                    customer.Fax = rdr["Fax"] != DBNull.Value ? rdr["Fax"].ToString() : null;
                    customers.Add(customer);
                }
            }
            return customers;
        }

        public List<Customer> SearchCustomers(string searchTerm)
        {
            var customers = new List<Customer>();
            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM Customers WHERE CompanyName LIKE @Search OR ContactName LIKE @Search", cn);
                cmd.Parameters.AddWithValue("@Search", $"%{searchTerm}%");
                cn.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var customer = new Customer();
                    customer.CustomerId = rdr["CustomerID"].ToString();
                    customer.CompanyName = rdr["CompanyName"].ToString();
                    customers.Add(customer);
                }
            }
            return customers;
        }

        public (List<Customer> Items, int TotalCount) GetCustomersPaged(int page, int pageSize)
        {
            var customers = new List<Customer>();
            int totalCount = 0;

            using (var cn = GlobalApplicationHelper.GetDbConnection())
            {
                var countCmd = new SqlCommand("SELECT COUNT(*) FROM Customers", cn);
                cn.Open();
                totalCount = (int)countCmd.ExecuteScalar();

                var cmd = new SqlCommand(@"
                    SELECT * FROM Customers
                    ORDER BY CompanyName
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", cn);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var customer = new Customer();
                    customer.CustomerId = rdr["CustomerID"].ToString();
                    customer.CompanyName = rdr["CompanyName"].ToString();
                    customer.ContactName = rdr.IsDBNull("ContactName") ? null : rdr["ContactName"].ToString();
                    customer.ContactTitle = rdr.IsDBNull("ContactTitle") ? null : rdr["ContactTitle"].ToString();
                    customer.City = rdr.IsDBNull("City") ? null : rdr["City"].ToString();
                    customer.Country = rdr.IsDBNull("Country") ? null : rdr["Country"].ToString();
                    customer.Phone = rdr.IsDBNull("Phone") ? null : rdr["Phone"].ToString();
                    customers.Add(customer);
                }
            }
            return (customers, totalCount);
        }
    }
}
