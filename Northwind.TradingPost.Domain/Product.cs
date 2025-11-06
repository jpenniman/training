namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents a product in the Northwind system.
    /// </summary>
    public class Product
    {
        private int _productId;
        private string _productName;
        private int? _supplierId;
        private int? _categoryId;
        private string _quantityPerUnit;
        private decimal? _unitPrice;
        private short? _unitsInStock;
        private short? _unitsOnOrder;
        private short? _reorderLevel;
        private bool _discontinued;

        /// <summary>
        /// Gets or sets the product ID.
        /// </summary>
        public int ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        /// <summary>
        /// Gets or sets the supplier ID.
        /// </summary>
        public int? SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }

        /// <summary>
        /// Gets or sets the category ID.
        /// </summary>
        public int? CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        /// <summary>
        /// Gets or sets the quantity per unit.
        /// </summary>
        public string QuantityPerUnit
        {
            get { return _quantityPerUnit; }
            set { _quantityPerUnit = value; }
        }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        public decimal? UnitPrice
        {
            get { return _unitPrice; }
            set { _unitPrice = value; }
        }

        /// <summary>
        /// Gets or sets the units in stock.
        /// </summary>
        public short? UnitsInStock
        {
            get { return _unitsInStock; }
            set { _unitsInStock = value; }
        }

        /// <summary>
        /// Gets or sets the units on order.
        /// </summary>
        public short? UnitsOnOrder
        {
            get { return _unitsOnOrder; }
            set { _unitsOnOrder = value; }
        }

        /// <summary>
        /// Gets or sets the reorder level.
        /// </summary>
        public short? ReorderLevel
        {
            get { return _reorderLevel; }
            set { _reorderLevel = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product is discontinued.
        /// </summary>
        public bool Discontinued
        {
            get { return _discontinued; }
            set { _discontinued = value; }
        }
    }
}