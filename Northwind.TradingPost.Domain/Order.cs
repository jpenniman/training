using System;

namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents an order in the Northwind system.
    /// </summary>
    public class Order
    {
        private int _orderId;
        private string _customerId;
        private int? _employeeId;
        private DateTime? _orderDate;
        private DateTime? _requiredDate;
        private DateTime? _shippedDate;
        private int? _shipVia;
        private decimal? _freight;
        private string _shipName;
        private string _shipAddress;
        private string _shipCity;
        private string _shipRegion;
        private string _shipPostalCode;
        private string _shipCountry;

        /// <summary>
        /// Gets or sets the order ID.
        /// </summary>
        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        /// <summary>
        /// Gets or sets the customer ID.
        /// </summary>
        public string CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public int? EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }

        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        public DateTime? OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        /// <summary>
        /// Gets or sets the required date.
        /// </summary>
        public DateTime? RequiredDate
        {
            get { return _requiredDate; }
            set { _requiredDate = value; }
        }

        /// <summary>
        /// Gets or sets the shipped date.
        /// </summary>
        public DateTime? ShippedDate
        {
            get { return _shippedDate; }
            set { _shippedDate = value; }
        }

        /// <summary>
        /// Gets or sets the ship via (shipper ID).
        /// </summary>
        public int? ShipVia
        {
            get { return _shipVia; }
            set { _shipVia = value; }
        }

        /// <summary>
        /// Gets or sets the freight.
        /// </summary>
        public decimal? Freight
        {
            get { return _freight; }
            set { _freight = value; }
        }

        /// <summary>
        /// Gets or sets the ship name.
        /// </summary>
        public string ShipName
        {
            get { return _shipName; }
            set { _shipName = value; }
        }

        /// <summary>
        /// Gets or sets the ship address.
        /// </summary>
        public string ShipAddress
        {
            get { return _shipAddress; }
            set { _shipAddress = value; }
        }

        /// <summary>
        /// Gets or sets the ship city.
        /// </summary>
        public string ShipCity
        {
            get { return _shipCity; }
            set { _shipCity = value; }
        }

        /// <summary>
        /// Gets or sets the ship region.
        /// </summary>
        public string ShipRegion
        {
            get { return _shipRegion; }
            set { _shipRegion = value; }
        }

        /// <summary>
        /// Gets or sets the ship postal code.
        /// </summary>
        public string ShipPostalCode
        {
            get { return _shipPostalCode; }
            set { _shipPostalCode = value; }
        }

        /// <summary>
        /// Gets or sets the ship country.
        /// </summary>
        public string ShipCountry
        {
            get { return _shipCountry; }
            set { _shipCountry = value; }
        }
    }
}