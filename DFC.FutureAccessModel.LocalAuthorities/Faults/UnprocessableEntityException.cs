using System.Diagnostics.CodeAnalysis;

namespace DFC.FutureAccessModel.LocalAuthorities.Faults
{
    /// <summary>
    /// unprocessable entity exception
    /// constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly
    /// </summary>    
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
    }
}
