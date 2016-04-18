using System.Collections.Generic;
using System.Xml.Linq;

namespace Hyper.Db.Xml
{
    /// <summary>
    /// Describes a class which represents a custom database schema configuration for a set of databases.
    /// </summary>
    public interface IXmlDbObjectCollection
    {
        /// <summary>
        /// Deserializes the specified <see cref="XElement"/> object into this instance.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the XML to deserialize.</param>
        void Deserialize(XElement parent);

        /// <summary>
        /// Gets a list of custom <see cref="IDbObject"/> objects belonging to the database described by this <see cref="IXmlDbObjectCollection"/> object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDbObject> GetDbObjects();
    }
}
