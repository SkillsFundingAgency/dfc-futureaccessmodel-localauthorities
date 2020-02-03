using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.FutureAccessModel.LocalAuthorities.Faults
{
    /// <summary>
    /// access forbidden exception
    /// constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ConflictingResourceException :
        Exception
    {
        /// <summary>
        /// the exception message
        /// </summary>
        public const string ExceptionMessage = "Resource already exists";

        /// <summary>
        /// initialises an instance of the <see cref="AccessForbiddenException"/>
        /// </summary>
        public ConflictingResourceException() :
            base(ExceptionMessage)
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="AccessForbiddenException"/>
        /// </summary>
        /// <param name="message"></param>
        public ConflictingResourceException(string message) :
            base(ExceptionMessage)
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="AccessForbiddenException"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConflictingResourceException(string message, Exception innerException) :
            base(ExceptionMessage, innerException)
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="AccessForbiddenException"/>
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected ConflictingResourceException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
