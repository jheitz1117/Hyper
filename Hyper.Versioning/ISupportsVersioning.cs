using System;

namespace Hyper.Versioning
{
    /// <summary>
    /// Defines a <see cref="System.Version"/> property for use in versioning logic.
    /// </summary>
    public interface ISupportsVersioning
    {
        /// <summary>
        /// The <see cref="System.Version"/> of the object.
        /// </summary>
        Version Version { get; set; }
    }
}
