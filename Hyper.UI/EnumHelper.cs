using System;
using System.ComponentModel;

namespace Hyper.UI
{
    public static class EnumHelper
    {
        #region Public Methods

        public static string GetDescription(Enum en)
        {
            var description = en.ToString();
            var type = en.GetType();
            
            var memInfo = type.GetMember(en.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }

        public static string GetDescription<TEnum>(int en)
        {
            if (!typeof(TEnum).IsEnum)
            { throw new ArgumentException("TEnum must be an enumerated type."); }

            var cust = (Enum)Enum.Parse(typeof(TEnum), en.ToString());
            return GetDescription(cust);
        }

        #endregion Public Methods
    }
}
