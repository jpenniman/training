using Microsoft.AspNetCore.Mvc;
using Northwind.TradingPost.Services;
using Northwind.TradingPost.Domain;
using System.Collections.Generic;

namespace Northwind.TradingPost.Web.Controllers
{
    public class OrdersController : Controller
    {
        private OrderService _orderService;
        private CustomerService _customerService;
        private ProductService _productService;

        public OrdersController()
        {
            _orderService = new OrderService();
            _customerService = new CustomerService();
            _productService = new ProductService();
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = _orderService.GetOrdersPaged(page, pageSize);
            
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            
            return View(result.Items);
        }

        public IActionResult Details(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            var details = _orderService.GetOrderDetails(id);
            ViewBag.OrderDetails = details;
            return View(order);
        }

        public IActionResult Create()
        {
            ViewBag.Customers = _customerService.SearchCustomers("");
            ViewBag.Products = _productService.GetProductsPaged(1, 100).Items;
            ViewBag.Shippers = GetAllShippers();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order, List<OrderDetail> orderDetails)
        {
            if (orderDetails == null)
                orderDetails = new List<OrderDetail>();

            if (string.IsNullOrEmpty(order.CustomerId))
            {
                ModelState.AddModelError("", "Customer is required");
                ViewBag.Customers = _customerService.SearchCustomers("");
                ViewBag.Products = _productService.GetProductsPaged(1, 100).Items;
                ViewBag.Shippers = GetAllShippers();
                return View(order);
            }

            if (!_orderService.ValidateOrder(order))
            {
                ModelState.AddModelError("", "Invalid order data");
                ViewBag.Customers = _customerService.SearchCustomers("");
                ViewBag.Products = _productService.GetProductsPaged(1, 100).Items;
                ViewBag.Shippers = GetAllShippers();
                return View(order);
            }

            if (orderDetails.Count == 0)
            {
                ModelState.AddModelError("", "At least one order item is required");
                ViewBag.Customers = _customerService.SearchCustomers("");
                ViewBag.Products = _productService.GetProductsPaged(1, 100).Items;
                ViewBag.Shippers = GetAllShippers();
                return View(order);
            }

            bool result = _orderService.CreateOrder(order, orderDetails);

            if (result)
                return RedirectToAction("Index");

            ViewBag.Customers = _customerService.SearchCustomers("");
            ViewBag.Products = _productService.GetProductsPaged(1, 100).Items;
            ViewBag.Shippers = GetAllShippers();
            return View(order);
        }

        public IActionResult Edit(int id, int detailsPage = 1, int detailsPageSize = 10, string activeTab = "order")
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            if (detailsPage < 1) detailsPage = 1;
            if (detailsPageSize < 1) detailsPageSize = 5;

            var result = _orderService.GetOrderDetailsPaged(id, detailsPage, detailsPageSize);
            
            ViewBag.DetailsCurrentPage = detailsPage;
            ViewBag.DetailsPageSize = detailsPageSize;
            ViewBag.DetailsTotalItems = result.TotalCount;
            ViewBag.DetailsTotalPages = (int)Math.Ceiling((double)result.TotalCount / detailsPageSize);
            ViewBag.OrderDetails = result.Items;
            ViewBag.ActiveTab = activeTab;

            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(Order order)
        {
            bool result = _orderService.UpdateOrder(order);

            if (result)
                return RedirectToAction("Index");

            return View(order);
        }

        public IActionResult Delete(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool result = _orderService.CancelOrder(id);

            if (result)
                return RedirectToAction("Index");

            return View();
        }

        public IActionResult ByCustomer(string customerId)
        {
            var orders = _orderService.GetOrdersByCustomer(customerId);
            return View("Index", orders);
        }

        public IActionResult ByDateRange(DateTime startDate, DateTime endDate)
        {
            var orders = _orderService.GetOrdersByDateRange(startDate, endDate);
            return View("Index", orders);
        }

        public IActionResult Ship(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            ViewBag.Shippers = GetAllShippers();
            return View(order);
        }

        [HttpPost]
        public IActionResult Ship(int orderId, int shipperId, string trackingNumber)
        {
            bool result = _orderService.ProcessShipment(orderId, shipperId, trackingNumber);

            if (result)
                return RedirectToAction("Index");

            return View();
        }

        private List<Shipper> GetAllShippers()
        {
            var shippers = new List<Shipper>();
            using (var cn = Northwind.TradingPost.Common.GlobalApplicationHelper.GetDbConnection())
            {
                var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM Shippers", cn);
                cn.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    shippers.Add(new Shipper
                    {
                        ShipperId = Convert.ToInt32(rdr["ShipperID"]),
                        CompanyName = rdr["CompanyName"].ToString()
                    });
                }
            }
            return shippers;
        }
    }
}
