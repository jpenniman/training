namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents a product category in the Northwind system.
    /// </summary>
    public class Category
    {
        private int _categoryId;
        private string _categoryName;
        private string _description;
        private byte[] _picture;

        /// <summary>
        /// Gets or sets the category ID.
        /// </summary>
        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        public string CategoryName
        {
            get { return _categoryName; }
            set { _categoryName = value; }
        }

        /// <summary>
        /// Gets or sets the category description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the category picture.
        /// </summary>
        public byte[] Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }
    }
}