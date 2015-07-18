namespace Hyper.FileProcessing.FixedWidthFiles
{
    public abstract class FixedWidthFieldBase : IFixedWidthField
    {
        /// <summary>
        /// Specifies the fixed length of the field
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Indicates whether the field is justified left or right
        /// </summary>
        public virtual FixedWidthJustifyType JustifyType
        {
            get { return FixedWidthJustifyType.Left; }
        }

        /// <summary>
        /// Specifies the character to use when padding this field in the event it is shorter than the Length property
        /// </summary>
        public virtual char PaddingChar
        {
            get { return _paddingChar; }
            protected set { _paddingChar = value; }
        } private char _paddingChar = ' ';

        /// <summary>
        /// Returns the raw value represented by this instance. The return type must override ToString() when possible
        /// to ensure the correct value is inserted into the file.
        /// </summary>
        /// <returns></returns>
        protected abstract object GetValue(); // We do this instead of a virtual or abstract property because I want strong typing for the Value properties on the child classes

        /// <summary>
        /// Returns the value represented by this instance as a string formatted for inclusion in a fixed-width file.
        /// Any overrides for this method may include custom logic to pad the field as necessary.
        /// </summary>
        /// <returns></returns>
        public string GetStringForFile()
        {
            return FixedWidthHelper.JustifyString((GetValue() ?? "").ToString(), Length, PaddingChar, JustifyType);
        }
    }
}
