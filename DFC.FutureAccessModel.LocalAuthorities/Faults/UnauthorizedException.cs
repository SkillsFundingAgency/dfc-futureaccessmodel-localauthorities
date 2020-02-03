using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.FutureAccessModel.LocalAuthorities.Faults
{
    /// <summary>
    /// the unauthorised exception
    /// constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnauthorizedException :
        Exception
    {
        /// <summary>
        /// the exception message
        /// </summary>
        public const string ExceptionMessage = "";

        /// <summary>
        /// initialises an instance of the <see cref="UnauthorizedException"/>
        /// </summary>
        public UnauthorizedException() :
            base(ExceptionMessage)
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="UnauthorizedException"/>
        /// </summary>
        /// <param name="message">message</param>
        public UnauthorizedException(string message) :
            base(ExceptionMessage)
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="UnauthorizedException"/>
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="innerException">inner exception</param>
        public UnauthorizedException(string message, Exception innerException) :
            base(ExceptionMessage, innerException)
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="UnauthorizedException"/>
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected UnauthorizedException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
