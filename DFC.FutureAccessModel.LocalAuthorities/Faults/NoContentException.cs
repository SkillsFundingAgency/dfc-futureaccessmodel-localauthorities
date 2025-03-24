using System.Diagnostics.CodeAnalysis;

namespace DFC.FutureAccessModel.LocalAuthorities.Faults
{
    /// <summary>
    /// no content exception
    /// constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly
    /// </summary>    
    [ExcludeFromCodeCoverage]
    public class NoContentException :
            Exception
    {
        /// <summary>
        /// initialises an instance of the <see cref="NoContentException"/>
        /// </summary>
        public NoContentException() :
            base(GetMessage())
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="NoContentException"/>
        /// </summary>
        /// <param name="message">message</param>
        public NoContentException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// initialises an instance of the <see cref="NoContentException"/>
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="innerException">inner exception</param>
        public NoContentException(string message, Exception innerException) :
            base(message, innerException)
        {
        }        

        /// <summary>
        /// get message
        /// </summary>
        /// <param name="parentResourceName">the parent resource name</param>
        /// <returns>the exception message</returns>
        public static string GetMessage(string parentResourceName = null) =>
            parentResourceName != null
                ? $"'{parentResourceName}' does not exist"
                : "Resource does not exist";
    }
}
