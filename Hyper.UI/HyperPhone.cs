using System.Text.RegularExpressions;

namespace Hyper.UI
{
    public class HyperPhone
    {
        #region Properties

        public long? PhoneNumber
        {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public HyperPhone() { }

        public HyperPhone(long? phoneNumber)
            : this()
        {
            PhoneNumber = phoneNumber;
        }

        public override string ToString()
        {
            if (PhoneNumber == null)
                return "";

            var formatString = "000-0000";
            if (PhoneNumber > 9999999999)
                formatString = "0 (000) 000-0000";
            else if (PhoneNumber > 9999999)
                formatString = "(000) 000-0000";

            return ((long)PhoneNumber).ToString(formatString);
        }

        public static HyperPhone Parse(string phoneNumberString)
        {
            var phoneInfo = new HyperPhone();

            phoneNumberString = Regex.Replace(phoneNumberString, "[^0-9]", "");

            if (!string.IsNullOrWhiteSpace(phoneNumberString))
            {
                long phoneNumber;
                long.TryParse(phoneNumberString, out phoneNumber);
                phoneInfo.PhoneNumber = phoneNumber;
            }

            return phoneInfo;
        }

        #endregion Public Methods
    }
}
