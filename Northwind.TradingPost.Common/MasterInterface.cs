using System.Data;

namespace Northwind.TradingPost.Common;

public class MasterInterface
{
    public object GetCustomerById(string id) => GlobalApplicationHelper.GetDbConnection();
    public object GetOrderById(int id) => null;
    public object GetProductById(int id) => null;
    public object GetEmployeeById(int id) => null;
    public object GetShipperById(int id) => null;
    public object GetSupplierById(int id) => null;
    public object GetCategoryById(int id) => null;
    public object GetTerritoryById(string id) => null;
    public object GetRegionById(int id) => null;
    public object GetOrderDetailByIds(int orderId, int productId) => null;
    public bool AddCustomer(object customer) => true;
    public bool AddOrder(object order) => true;
    public bool AddProduct(object product) => true;
    public bool AddEmployee(object employee) => true;
    public bool AddShipper(object shipper) => true;
    public bool AddSupplier(object supplier) => true;
    public bool AddCategory(object category) => true;
    public bool AddTerritory(object territory) => true;
    public bool AddRegion(object region) => true;
    public bool AddOrderDetail(object orderDetail) => true;
    public bool UpdateCustomer(object customer) => true;
    public bool UpdateOrder(object order) => true;
    public bool UpdateProduct(object product) => true;
    public bool UpdateEmployee(object employee) => true;
    public bool UpdateShipper(object shipper) => true;
    public bool UpdateSupplier(object supplier) => true;
    public bool UpdateCategory(object category) => true;
    public bool UpdateTerritory(object territory) => true;
    public bool UpdateRegion(object region) => true;
    public bool UpdateOrderDetail(object orderDetail) => true;
    public bool DeleteCustomer(string id) => true;
    public bool DeleteOrder(int id) => true;
    public bool DeleteProduct(int id) => true;
    public bool DeleteEmployee(int id) => true;
    public bool DeleteShipper(int id) => true;
    public bool DeleteSupplier(int id) => true;
    public bool DeleteCategory(int id) => true;
    public bool DeleteTerritory(string id) => true;
    public bool DeleteRegion(int id) => true;
    public bool DeleteOrderDetail(int orderId, int productId) => true;
    public DataTable GetAllCustomers() => new DataTable();
    public DataTable GetAllOrders() => new DataTable();
    public DataTable GetAllProducts() => new DataTable();
    public DataTable GetAllEmployees() => new DataTable();
    public DataTable GetAllShippers() => new DataTable();
    public DataTable GetAllSuppliers() => new DataTable();
    public DataTable GetAllCategories() => new DataTable();
    public DataTable GetAllTerritories() => new DataTable();
    public DataTable GetAllRegions() => new DataTable();
    public DataTable GetAllOrderDetails() => new DataTable();
}