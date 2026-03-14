using Microsoft.AspNetCore.Mvc;
using Northwind.TradingPost.Services;
using Northwind.TradingPost.Domain;
using System.Collections.Generic;

namespace Northwind.TradingPost.Web.Controllers
{
    public class ProductsController : Controller
    {
        private ProductService _productService;
        private ReportingService _reportingService;

        public ProductsController()
        {
            _productService = new ProductService();
            _reportingService = new ReportingService();
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = _productService.GetProductsPaged(page, pageSize);
            
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            
            return View(result.Items);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!_productService.ValidateProduct(product))
            {
                ModelState.AddModelError("", "Invalid product data");
                return View(product);
            }

            bool result = _productService.AddProduct(product);

            if (result)
                return RedirectToAction("Index");

            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();

            var categories = _productService.GetCategories();
            ViewBag.Categories = categories;

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            bool result = _productService.UpdateProduct(product);

            if (result)
                return RedirectToAction("Index");

            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            bool result = _productService.DeleteProduct(id);

            if (result)
                return RedirectToAction("Index");

            return View();
        }

        public IActionResult ByCategory(int categoryId)
        {
            var products = _productService.GetProductsByCategory(categoryId);
            return View("Index", products);
        }

        public IActionResult LowStock()
        {
            var products = _productService.GetLowStockProducts(10);
            return View(products);
        }

        public IActionResult Top10()
        {
            var products = _productService.GetTop10ExpensiveProducts();
            ViewBag.Products = products;
            return View();
        }

        public IActionResult Discontinued()
        {
            var inventoryService = new InventoryService();
            var products = inventoryService.GetDiscontinuedProducts();
            return View(products);
        }

        public IActionResult AdjustStock(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult AdjustStock(int productId, int newQuantity, string reason)
        {
            var inventoryService = new InventoryService();
            bool result = inventoryService.AdjustStock(productId, newQuantity, reason);

            if (result)
                return RedirectToAction("Details", new { id = productId });

            return View();
        }
    }
}
