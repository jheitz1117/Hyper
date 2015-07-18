using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Hyper.Db
{
    /// <summary>
    /// Describes a class which represents a custom database schema configuration for a set of databases.
    /// </summary>
    public interface IDbCustomConfiguration
    {
        /// <summary>
        /// Deserializes the specified <see cref="XElement"/> object into this instance.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> object containing the XML to deserialize.</param>
        /// <param name="scriptWriterTypes">The list of <see cref="IDbScriptWriter"/> types used in the parent schema.</param>
        void Deserialize(XElement parent, IDictionary<string, Type> scriptWriterTypes);

        /// <summary>
        /// Gets a list of custom <see cref="IDbObject"/> objects belonging to the database described by this <see cref="IDbCustomConfiguration"/> object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDbObject> GetDbObjects();
    }
}
