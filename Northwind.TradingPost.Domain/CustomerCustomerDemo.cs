namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents a mapping between a customer and a customer demographic in the Northwind system.
    /// </summary>
    public class CustomerCustomerDemo
    {
        private string _customerId;
        private string _customerTypeId;

        /// <summary>
        /// Gets or sets the customer ID.
        /// </summary>
        public string CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }

        /// <summary>
        /// Gets or sets the customer type ID.
        /// </summary>
        public string CustomerTypeId
        {
            get { return _customerTypeId; }
            set { _customerTypeId = value; }
        }
    }
}