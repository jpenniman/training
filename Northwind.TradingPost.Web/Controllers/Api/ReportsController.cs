using Microsoft.AspNetCore.Mvc;
using Northwind.TradingPost.Services;
using System;

namespace Northwind.TradingPost.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private ReportingService _reportingService;

        public ReportsController()
        {
            _reportingService = new ReportingService();
        }

        [HttpGet("sales-by-year")]
        public IActionResult GetSalesByYear([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var data = _reportingService.GetSalesByYearReport(startDate, endDate);
            return Ok(data);
        }

        [HttpGet("sales-by-category")]
        public IActionResult GetSalesByCategory([FromQuery] string categoryName, [FromQuery] string ordYear = "1996")
        {
            var data = _reportingService.GetSalesByCategoryReport(categoryName, ordYear);
            return Ok(data);
        }

        [HttpGet("top-products")]
        public IActionResult GetTopProducts([FromQuery] int topN = 10)
        {
            var data = _reportingService.GetTopProductsReport(topN);
            return Ok(data);
        }

        [HttpGet("employee-sales")]
        public IActionResult GetEmployeeSales([FromQuery] int? employeeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var data = _reportingService.GetEmployeeSalesReport(employeeId, startDate, endDate);
            return Ok(data);
        }

        [HttpGet("category-sales")]
        public IActionResult GetCategorySales()
        {
            var data = _reportingService.GetCategorySalesReport();
            return Ok(data);
        }

        [HttpGet("shipper-performance")]
        public IActionResult GetShipperPerformance()
        {
            var data = _reportingService.GetShipperPerformanceReport();
            return Ok(data);
        }

        [HttpGet("customer-lifetime-value")]
        public IActionResult GetCustomerLifetimeValue()
        {
            var data = _reportingService.GetCustomerLifetimeValueReport();
            return Ok(data);
        }

        [HttpGet("inventory-value")]
        public IActionResult GetInventoryValue()
        {
            var data = _reportingService.GetInventoryValueReport();
            return Ok(data);
        }

        [HttpGet("monthly-trend")]
        public IActionResult GetMonthlyTrend([FromQuery] int year = 1996)
        {
            var data = _reportingService.GetMonthlySalesTrendReport(year);
            return Ok(data);
        }
    }
}
