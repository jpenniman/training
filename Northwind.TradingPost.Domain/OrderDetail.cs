namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents an order detail in the Northwind system.
    /// </summary>
    public class OrderDetail
    {
        private int _orderId;
        private int _productId;
        private string _productName;
        private decimal _unitPrice;
        private short _quantity;
        private float _discount;

        /// <summary>
        /// Gets or sets the order ID.
        /// </summary>
        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

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
        /// Gets or sets the unit price.
        /// </summary>
        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set { _unitPrice = value; }
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public short Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        public float Discount
        {
            get { return _discount; }
            set { _discount = value; }
        }
    }
}