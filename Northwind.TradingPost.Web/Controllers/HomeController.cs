using Microsoft.AspNetCore.Mvc;
using Northwind.TradingPost.Services;
using Northwind.TradingPost.Common;

namespace Northwind.TradingPost.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Version = GlobalApplicationHelper.GetAppVersion();
            ViewBag.IsProduction = GlobalApplicationHelper.IsProduction();
            
            var reportingService = new ReportingService();
            ViewBag.TopProducts = reportingService.GetTopProductsReport(5);
            
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            var orderService = new OrderService();
            var productService = new ProductService();
            var customerService = new CustomerService();
            
            ViewBag.RecentOrders = orderService.GetOrdersByDateRange(DateTime.Now.AddDays(-30), DateTime.Now);
            ViewBag.LowStockProducts = productService.GetLowStockProducts(10);
            
            return View();
        }

        public IActionResult Reports()
        {
            var reportingService = new ReportingService();
            
            ViewBag.SalesByYear = reportingService.GetSalesByYearReport(DateTime.Now.AddYears(-1), DateTime.Now);
            ViewBag.TopProducts = reportingService.GetTopProductsReport(10);
            ViewBag.CategorySales = reportingService.GetCategorySalesReport();
            
            return View();
        }
    }
}
