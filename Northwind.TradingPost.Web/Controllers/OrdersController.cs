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
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (!_orderService.ValidateOrder(order))
            {
                ModelState.AddModelError("", "Invalid order data");
                return View(order);
            }

            var details = new List<OrderDetail>();
            bool result = _orderService.CreateOrder(order, details);

            if (result)
                return RedirectToAction("Index");

            return View(order);
        }

        public IActionResult Edit(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

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
