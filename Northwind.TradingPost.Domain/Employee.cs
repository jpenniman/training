using System;

namespace Northwind.TradingPost.Domain
{
    /// <summary>
    /// Represents an employee in the Northwind system.
    /// </summary>
    public class Employee
    {
        private int _employeeId;
        private string _lastName;
        private string _firstName;
        private string _title;
        private string _titleOfCourtesy;
        private DateTime? _birthDate;
        private DateTime? _hireDate;
        private string _address;
        private string _city;
        private string _region;
        private string _postalCode;
        private string _country;
        private string _homePhone;
        private string _extension;
        private byte[] _photo;
        private string _notes;
        private int? _reportsTo;
        private string _photoPath;

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public int EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the title of courtesy.
        /// </summary>
        public string TitleOfCourtesy
        {
            get { return _titleOfCourtesy; }
            set { _titleOfCourtesy = value; }
        }

        /// <summary>
        /// Gets or sets the birth date.
        /// </summary>
        public DateTime? BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; }
        }

        /// <summary>
        /// Gets or sets the hire date.
        /// </summary>
        public DateTime? HireDate
        {
            get { return _hireDate; }
            set { _hireDate = value; }
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        /// <summary>
        /// Gets or sets the home phone.
        /// </summary>
        public string HomePhone
        {
            get { return _homePhone; }
            set { _homePhone = value; }
        }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        public byte[] Photo
        {
            get { return _photo; }
            set { _photo = value; }
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        /// <summary>
        /// Gets or sets the ID of the employee this employee reports to.
        /// </summary>
        public int? ReportsTo
        {
            get { return _reportsTo; }
            set { _reportsTo = value; }
        }

        /// <summary>
        /// Gets or sets the photo path.
        /// </summary>
        public string PhotoPath
        {
            get { return _photoPath; }
            set { _photoPath = value; }
        }
    }
}