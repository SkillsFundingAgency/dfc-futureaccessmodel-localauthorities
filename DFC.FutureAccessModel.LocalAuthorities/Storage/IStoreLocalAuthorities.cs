using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Registration;

namespace DFC.FutureAccessModel.LocalAuthorities.Storage
{
    /// <summary>
    /// i store local authorities
    /// </summary>
    public interface IStoreLocalAuthorities :
        ISupportServiceRegistration
    {
        /// <summary>
        /// get...
        /// </summary>
        /// <param name="theAdminDistrict">the admin distict (code)</param>
        /// <returns>a local authority</returns>
        Task<ILocalAuthority> Get(string theAdminDistrict);

        /// <summary>
        /// add...
        /// </summary>
        /// <param name="theCandidate">the candidate</param>
        /// <returns>the newly added local authority</returns>
        Task<ILocalAuthority> Add(IncomingLocalAuthority theCandidate);

        /// <summary>
        /// delete...
        /// </summary>
        /// <param name="theAdminDistrict">the admin distict (code)</param>
        /// <returns>the currently running task</returns>
        Task Delete(string theAdminDistrict);
    }
}