using System.ComponentModel.DataAnnotations;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories
{
    /// <summary>
    /// i create validation messages
    /// </summary>
    public interface ICreateValidationMessageContent
    {
        /// <summary>
        /// create... transforms a collection of <seealso cref="ValidationResult"/>
        /// </summary>
        /// <param name="errors">the collection of errors</param>
        /// <returns>the validation message string</returns>
        string Create(IReadOnlyCollection<ValidationResult> errors);
    }
}