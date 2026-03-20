using Northwind.TradingPost.Common;
using Northwind.TradingPost.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Northwind.TradingPost.Services;

public class WorkflowService
{
    public bool ProcessCompleteOrderWorkflow(Order order, List<OrderDetail> details, string paymentCardNumber, string paymentExpiry)
    {
        var orderService = new OrderService();
        var productService = new ProductService();

        foreach (var detail in details)
        {
            var product = productService.GetProductById(detail.ProductId);
            if (product == null || product.UnitsInStock < detail.Quantity)
            {
                GlobalApplicationHelper.LogError(new Exception($"Insufficient stock for product {detail.ProductId}"), "Workflow");
                return false;
            }
        }

        if (!GlobalApplicationHelper.ProcessPayment(order.CustomerId, 
                orderService.CalculateOrderTotal(order, details), paymentCardNumber, paymentExpiry))
        {
            return false;
        }

        if (!orderService.CreateOrder(order, details))
        {
            return false;
        }

        return true;
    }

    public bool ProcessOrderCancellationWorkflow(int orderId, string reason)
    {
        var orderService = new OrderService();
        var order = orderService.GetOrderById(orderId);
            
        if (order == null)
            return false;

        if (order.ShippedDate.HasValue)
        {
            GlobalApplicationHelper.LogError(new Exception($"Cannot cancel shipped order {orderId}"), "CancelWorkflow");
            return false;
        }

        return orderService.CancelOrder(orderId);
    }

    public bool ProcessReturnWorkflow(int orderId, int productId, int quantity)
    {
        var orderService = new OrderService();
        var productService = new ProductService();
        var details = orderService.GetOrderDetails(orderId);

        var detail = details.FirstOrDefault(d => d.ProductId == productId);
        if (detail == null)
            return false;

        if (quantity > detail.Quantity)
            quantity = detail.Quantity;

        productService.UpdateStock(productId, quantity);

        GlobalApplicationHelper.LogError(new Exception($"Return processed for order {orderId}, product {productId}, qty {quantity}"), "Return");

        return true;
    }
}