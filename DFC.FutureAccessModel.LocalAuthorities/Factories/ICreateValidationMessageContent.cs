using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DFC.FutureAccessModel.LocalAuthorities.Registration;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories
{
    /// <summary>
    /// i create validation messages
    /// </summary>
    public interface ICreateValidationMessageContent :
        ISupportServiceRegistration
    {
        /// <summary>
        /// create... transforms a collection of <seealso cref="ValidationResult"/>
        /// </summary>
        /// <param name="errors">the collection of errors</param>
        /// <returns>the validation message string</returns>
        string Create(IReadOnlyCollection<ValidationResult> errors);
    }
}