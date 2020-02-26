using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the  validation message factory
    /// </summary>
    internal sealed class ValidationMessageContentFactory :
        ICreateValidationMessageContent
    {
        /// <summary>
        /// create... transforms a collection of <seealso cref="ValidationResult"/>
        /// </summary>
        /// <param name="errors">the collection of errors</param>
        /// <returns>the validation message string</returns>
        public string Create(IReadOnlyCollection<ValidationResult> errors)
        {
            var message = new ValidationMessage();

            errors.ForEach(x => x.MemberNames.ForEach(y => message.Add(y, x.ErrorMessage)));

            return message.ToString();
        }
    }
}