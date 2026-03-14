using Microsoft.AspNetCore.Mvc;
using Northwind.TradingPost.Services;
using Northwind.TradingPost.Domain;
using System.Collections.Generic;

namespace Northwind.TradingPost.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private OrderService _orderService;

        public OrdersController()
        {
            _orderService = new OrderService();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _orderService.GetOrdersByCustomer("");
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            if (!_orderService.ValidateOrder(order))
                return BadRequest("Invalid order data");

            var details = new List<OrderDetail>();
            bool result = _orderService.CreateOrder(order, details);

            if (result)
                return CreatedAtAction(nameof(Get), new { id = order.OrderId }, order);

            return BadRequest("Failed to create order");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Order order)
        {
            order.OrderId = id;
            bool result = _orderService.UpdateOrder(order);

            if (result)
                return Ok(order);

            return BadRequest("Failed to update order");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool result = _orderService.CancelOrder(id);

            if (result)
                return NoContent();

            return BadRequest("Failed to cancel order");
        }

        [HttpGet("customer/{customerId}")]
        public IActionResult GetByCustomer(string customerId)
        {
            var orders = _orderService.GetOrdersByCustomer(customerId);
            return Ok(orders);
        }

        [HttpGet("daterange")]
        public IActionResult GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var orders = _orderService.GetOrdersByDateRange(startDate, endDate);
            return Ok(orders);
        }

        [HttpPost("{id}/ship")]
        public IActionResult Ship(int id, [FromBody] ShipRequest request)
        {
            bool result = _orderService.ProcessShipment(id, request.ShipperId, request.TrackingNumber);

            if (result)
                return Ok();

            return BadRequest("Failed to process shipment");
        }
    }

    public class ShipRequest
    {
        public int ShipperId { get; set; }
        public string TrackingNumber { get; set; }
    }
}
