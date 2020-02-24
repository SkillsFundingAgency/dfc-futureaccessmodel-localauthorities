using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Registration;

namespace DFC.FutureAccessModel.LocalAuthorities.Validation
{
    /// <summary>
    /// i validate local authorities
    /// </summary>
    public interface IValidateLocalAuthorities :
        ISupportServiceRegistration
    {
        /// <summary>
        /// validate...
        /// </summary>
        /// <param name="theCandidate">the candidate</param>
        /// <returns>the currently running task</returns>
        Task Validate(ILocalAuthority theCandidate);
    }
}