using System;

namespace Hyper.Versioning
{
    /// <summary>
    /// Describes a class which supports versioning using a <see cref="System.Version"/> object.
    /// </summary>
    public interface ISupportsVersioning
    {
        /// <summary>
        /// The <see cref="System.Version"/> of the object.
        /// </summary>
        Version Version { get; set; }
    }
}
