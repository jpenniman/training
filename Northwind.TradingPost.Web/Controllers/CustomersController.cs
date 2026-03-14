using Microsoft.AspNetCore.Mvc;
using Northwind.TradingPost.Services;
using Northwind.TradingPost.Domain;

namespace Northwind.TradingPost.Web.Controllers
{
    public class CustomersController : Controller
    {
        private CustomerService _customerService;

        public CustomersController()
        {
            _customerService = new CustomerService();
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            const int defaultPageSize = 10;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = defaultPageSize;

            var result = _customerService.GetCustomersPaged(page, pageSize);
            
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            
            return View(result.Items);
        }

        public IActionResult Details(string id)
        {
            var dao = new Northwind.TradingPost.DataAccess.CustomerDAO();
            var customer = dao.GetById(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (!_customerService.ValidateCustomer(customer))
            {
                ModelState.AddModelError("", "Invalid customer data");
                return View(customer);
            }

            bool result = _customerService.AddCustomer(customer);

            if (result)
                return RedirectToAction("Index");

            return View(customer);
        }

        public IActionResult Edit(string id)
        {
            var dao = new Northwind.TradingPost.DataAccess.CustomerDAO();
            var customer = dao.GetById(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            bool result = _customerService.UpdateCustomer(customer);

            if (result)
                return RedirectToAction("Index");

            return View(customer);
        }

        public IActionResult Delete(string id)
        {
            var dao = new Northwind.TradingPost.DataAccess.CustomerDAO();
            var customer = dao.GetById(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            bool result = _customerService.DeleteCustomer(id);

            if (result)
                return RedirectToAction("Index");

            return View();
        }

        public IActionResult ByCountry(string country)
        {
            var customers = _customerService.GetCustomersByCountry(country);
            return View("Index", customers);
        }

        public IActionResult ApplyDiscount(string customerId, decimal discount)
        {
            bool result = _customerService.ApplyDiscount(customerId, discount);
            
            if (result)
                TempData["Message"] = "Discount applied successfully";
            else
                TempData["Error"] = "Failed to apply discount";

            return RedirectToAction("Details", new { id = customerId });
        }

        public IActionResult OrderHistory(string id)
        {
            var orderService = new OrderService();
            var orders = orderService.GetOrdersByCustomer(id);
            ViewBag.CustomerId = id;
            return View(orders);
        }
    }
}
