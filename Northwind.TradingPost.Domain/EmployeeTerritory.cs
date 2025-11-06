namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents a mapping between an employee and a territory in the Northwind system.
    /// </summary>
    public class EmployeeTerritory
    {
        private int _employeeId;
        private string _territoryId;

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public int EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }

        /// <summary>
        /// Gets or sets the territory ID.
        /// </summary>
        public string TerritoryId
        {
            get { return _territoryId; }
            set { _territoryId = value; }
        }
    }
}