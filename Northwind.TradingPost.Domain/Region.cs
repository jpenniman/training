namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents a region in the Northwind system.
    /// </summary>
    public class Region
    {
        private int _regionId;
        private string _regionDescription;

        /// <summary>
        /// Gets or sets the region ID.
        /// </summary>
        public int RegionId
        {
            get { return _regionId; }
            set { _regionId = value; }
        }

        /// <summary>
        /// Gets or sets the region description.
        /// </summary>
        public string RegionDescription
        {
            get { return _regionDescription; }
            set { _regionDescription = value; }
        }
    }
}