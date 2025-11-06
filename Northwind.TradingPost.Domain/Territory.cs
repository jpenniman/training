namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents a territory in the Northwind system.
    /// </summary>
    public class Territory
    {
        private string _territoryId;
        private string _territoryDescription;
        private int _regionId;

        /// <summary>
        /// Gets or sets the territory ID.
        /// </summary>
        public string TerritoryId
        {
            get { return _territoryId; }
            set { _territoryId = value; }
        }

        /// <summary>
        /// Gets or sets the territory description.
        /// </summary>
        public string TerritoryDescription
        {
            get { return _territoryDescription; }
            set { _territoryDescription = value; }
        }

        /// <summary>
        /// Gets or sets the region ID.
        /// </summary>
        public int RegionId
        {
            get { return _regionId; }
            set { _regionId = value; }
        }
    }
}