namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents customer demographic information in the Northwind system.
    /// </summary>
    public class CustomerDemographic
    {
        private string _customerTypeId;
        private string _customerDesc;

        /// <summary>
        /// Gets or sets the customer type ID.
        /// </summary>
        public string CustomerTypeId
        {
            get { return _customerTypeId; }
            set { _customerTypeId = value; }
        }

        /// <summary>
        /// Gets or sets the customer description.
        /// </summary>
        public string CustomerDesc
        {
            get { return _customerDesc; }
            set { _customerDesc = value; }
        }
    }
}