namespace Northwind.TradingPost.Domain
{

    /// <summary>
    /// Represents a shipper in the Northwind system.
    /// </summary>
    public class Shipper
    {
        int _shipperId;
        string _companyName;
        string _phone;

        /// <summary>
        /// Gets or sets the shipper ID.
        /// </summary>
        public int ShipperId
        {
            get { return _shipperId; }
            set { _shipperId = value; }
        }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
    }
}
