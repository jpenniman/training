using Microsoft.AspNetCore.Mvc;
using Northwind.TradingPost.Services;
using Northwind.TradingPost.Domain;
using System.Collections.Generic;

namespace Northwind.TradingPost.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private CustomerService _customerService;

        public CustomersController()
        {
            _customerService = new CustomerService();
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string search = "")
        {
            var customers = _customerService.SearchCustomers(search);
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var dao = new Northwind.TradingPost.DataAccess.CustomerDAO();
            var customer = dao.GetById(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer customer)
        {
            if (!_customerService.ValidateCustomer(customer))
                return BadRequest("Invalid customer data");

            bool result = _customerService.AddCustomer(customer);

            if (result)
                return CreatedAtAction(nameof(Get), new { id = customer.CustomerId }, customer);

            return BadRequest("Failed to create customer");
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Customer customer)
        {
            customer.CustomerId = id;
            bool result = _customerService.UpdateCustomer(customer);

            if (result)
                return Ok(customer);

            return BadRequest("Failed to update customer");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            bool result = _customerService.DeleteCustomer(id);

            if (result)
                return NoContent();

            return BadRequest("Failed to delete customer");
        }

        [HttpGet("country/{country}")]
        public IActionResult GetByCountry(string country)
        {
            var customers = _customerService.GetCustomersByCountry(country);
            return Ok(customers);
        }

        [HttpPost("{id}/discount")]
        public IActionResult ApplyDiscount(string id, [FromBody] DiscountRequest request)
        {
            bool result = _customerService.ApplyDiscount(id, request.DiscountPercent);

            if (result)
                return Ok();

            return BadRequest("Failed to apply discount");
        }
    }

    public class DiscountRequest
    {
        public decimal DiscountPercent { get; set; }
    }
}
