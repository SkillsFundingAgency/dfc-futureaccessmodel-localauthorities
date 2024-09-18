using DFC.FutureAccessModel.LocalAuthorities.Models;

namespace DFC.FutureAccessModel.LocalAuthorities.Validation
{
    /// <summary>
    /// i validate local authorities
    /// </summary>
    public interface IValidateLocalAuthorities
    {
        /// <summary>
        /// validate...
        /// </summary>
        /// <param name="theCandidate">the candidate</param>
        /// <returns>the currently running task</returns>
        Task Validate(ILocalAuthority theCandidate);
    }
}