using System.Xml.Linq;

namespace Hyper.Db.Xml
{
    /// <summary>
    /// Provides XML serialization support for creating instances of <see cref="IDbSchemaCollection"/>.
    /// </summary>
    public interface IXmlDbSchemaCollection : IDbSchemaCollection
    {
        /// <summary>
        /// Deserializes the specified <see cref="XDocument"/> object into this instance.
        /// </summary>
        /// <param name="dbXmlDocument">The <see cref="XDocument"/> object containing the XML to deserialize.</param>
        void Deserialize(XDocument dbXmlDocument);
    }
}
