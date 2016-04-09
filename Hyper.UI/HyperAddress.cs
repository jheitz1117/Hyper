using System.Collections.Generic;
using System.Linq;

namespace Hyper.UI
{
    public class HyperAddress
    {
        #region Properties

        private List<string> _addressLines = new List<string>();
        public List<string> AddressLines
        {
            get
            {
                return _addressLines;
            }
            set
            {
                if (value == null)
                {
                    _addressLines = new List<string>();
                }

                _addressLines = value;
            }
        }

        public string City
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }

        public string ZipCode
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public HyperAddress() { }

        public HyperAddress(string addressLine1, string addressLine2, string city, string state, string zipCode)
            : this()
        {
            AddressLines.Add(addressLine1);
            AddressLines.Add(addressLine2);
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        public static string BuildCszLine(string city, string state, string zipCode)
        {
            var cszLine = "";

            if (!string.IsNullOrWhiteSpace(city))
                cszLine += city.Trim();
            if (!string.IsNullOrWhiteSpace(state))
                cszLine += ", " + state.Trim();
            if (!string.IsNullOrWhiteSpace(zipCode))
                cszLine += " " + zipCode.Trim();

            return cszLine.Trim(", ".ToCharArray());
        }

        public override string ToString()
        {
            return ToString("\n");
        }

        /// <summary>
        /// Condenses the lines for this HyperAddress object into a single line separated by commas
        /// </summary>
        /// <param name="forceSingleLine">Indicates whether to force the address lines into a single line.</param>
        /// <returns></returns>
        public string ToString(bool forceSingleLine)
        {
            return forceSingleLine ? ToString(", ") : ToString();
        }

        /// <summary>
        /// Returns the lines for this HyperAddress object using the specified line separator.
        /// </summary>
        /// <param name="lineSeparator">String to insert between address lines.</param>
        /// <returns></returns>
        public string ToString(string lineSeparator)
        {
            return string.Join(lineSeparator, GetCondensedAddressLines().ToArray());
        }

        #endregion Public Methods

        #region Private Methods

        private List<string> GetCondensedAddressLines()
        {
            var condensedAddressLines = (from addressLine in AddressLines where !string.IsNullOrWhiteSpace(addressLine) select addressLine.Trim()).ToList();

            var cszLine = BuildCszLine(City, State, ZipCode);
            if (!string.IsNullOrWhiteSpace(CountryCode))
                cszLine += " " + CountryCode.Trim();

            if (!string.IsNullOrWhiteSpace(cszLine))
                condensedAddressLines.Add(cszLine.Trim());

            return condensedAddressLines;
        }

        #endregion Private Methods
    }
}