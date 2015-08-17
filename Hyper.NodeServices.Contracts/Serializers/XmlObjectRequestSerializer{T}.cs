﻿using System;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    /// <summary>
    /// Abstract implementation of <see cref="XmlObjectSerializerWrapper{T}"/> that can serialize instances of <see cref="ICommandRequest"/>.
    /// </summary>
    /// <typeparam name="T">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    public abstract class XmlObjectRequestSerializer<T> : XmlObjectSerializerWrapper<T>, ICommandRequestSerializer
        where T : ICommandRequest
    {
        /// <summary>
        /// Serializes the specified <see cref="ICommandRequest"/> into a string.
        /// </summary>
        /// <param name="request">The <typeparamref name="T"/> instance to serialize. This instance must implement <see cref="ICommandRequest"/>.</param>
        /// <returns></returns>
        public string Serialize(ICommandRequest request)
        {
            return base.Serialize((T)request);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="requestString"/> into an object of type <typeparamref name="T" />.
        /// </summary>
        /// <param name="requestString">The string to deserialize.</param>
        /// <returns></returns>
        ICommandRequest ICommandRequestSerializer.Deserialize(string requestString)
        {
            return base.Deserialize(requestString);
        }

        /// <summary>
        /// Gets the expected <see cref="Type"/> of the <see cref="ICommandRequest"/> object.
        /// </summary>
        /// <returns></returns>
        public Type GetRequestType()
        {
            return typeof(T);
        }
    }
}
