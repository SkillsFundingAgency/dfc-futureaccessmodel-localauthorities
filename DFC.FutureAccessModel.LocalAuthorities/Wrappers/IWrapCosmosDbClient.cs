using DFC.FutureAccessModel.LocalAuthorities.Models;
using Microsoft.Azure.Cosmos;

namespace DFC.FutureAccessModel.LocalAuthorities.Wrappers
{
    public interface IWrapCosmosDbClient
    {
        Task<bool> LocalAuthorityExistsAsync(string id, string partitionKey);
        Task<LocalAuthority> GetLocalAuthorityAsync(string theAdminDistrict, string partitionKey);
        Task<ItemResponse<LocalAuthority>> CreateLocalAuthorityAsync(LocalAuthority localAuthority, string partitionKey);
        Task<ItemResponse<LocalAuthority>> DeleteLocalAuthorityAsync(string theAdminDistrict, string partitionKey);
    }
}