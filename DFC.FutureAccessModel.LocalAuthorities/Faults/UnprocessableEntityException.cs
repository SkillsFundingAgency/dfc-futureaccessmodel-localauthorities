using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.FutureAccessModel.LocalAuthorities.Faults
{
    /// <summary>
    /// unprocessable entity exception
    /// constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnprocessableEntityException :
            Exception
    {
        /// <summary>
        /// initialise an instance of the <see cref="UnprocessableEntityException"/>
        /// </summary>
        public UnprocessableEntityException() :
            base(string.Empty)
        {
        }

        /// <summary>
        /// initialise an instance of the <see cref="UnprocessableEntityException"/>
        /// </summary>
        /// <param name="message">message</param>
        public UnprocessableEntityException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// initialise an instance of the <see cref="UnprocessableEntityException"/>
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="innerException">inner exception</param>
        public UnprocessableEntityException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// initialise an instance of the <see cref="UnprocessableEntityException"/>
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected UnprocessableEntityException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
