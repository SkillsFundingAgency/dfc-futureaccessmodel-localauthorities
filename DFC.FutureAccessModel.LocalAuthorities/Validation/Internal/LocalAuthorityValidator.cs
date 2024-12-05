using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using System.ComponentModel.DataAnnotations;

namespace DFC.FutureAccessModel.LocalAuthorities.Validation.Internal
{
    /// <summary>
    /// the local authority validator
    /// </summary>
    internal sealed class LocalAuthorityValidator :
        IValidateLocalAuthorities
    {
        /// <summary>
        /// the message (factory)
        /// </summary>
        public ICreateValidationMessageContent Message { get; }

        /// <summary>
        /// initialsies an instance of <see cref="LocalAuthorityValidator"/>
        /// </summary>
        /// <param name="message">the message (factory)</param>
        public LocalAuthorityValidator(ICreateValidationMessageContent message)
        {
            It.IsNull(message)
                .AsGuard<ArgumentNullException>(nameof(message));

            Message = message;
        }

        /// <summary>
        /// validate...
        /// </summary>
        /// <param name="theCandidate">the candidate</param>
        /// <returns>the currently running task</returns>
        public async Task Validate(ILocalAuthority theCandidate) =>
            await Task.Run(() =>
            {
                It.IsNull(theCandidate)
                    .AsGuard<ArgumentNullException>(nameof(theCandidate));

                var context = new ValidationContext(theCandidate, null, null);
                var results = Collection.Empty<ValidationResult>();

                Validator.TryValidateObject(theCandidate, context, results, true);

                results.Any()
                    .AsGuard<UnprocessableEntityException>(Message.Create(results.AsSafeReadOnlyList()));
            });
    }
}
