using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Wrappers;

namespace DFC.FutureAccessModel.LocalAuthorities.Storage.Internal
{
    /// <summary>
    /// the area routing detail store
    /// </summary>
    internal sealed class LocalAuthorityStore :
        IStoreLocalAuthorities
    {
        const string _partitionKey = "not_required";

        /// <summary>
        /// cosmos db provider
        /// </summary>
        public IWrapCosmosDbClient CosmosDbWrapper { get; }

        /// <summary>
        /// create an instance of hte <see cref="LocalAuthorityStore"/>
        /// </summary>
        /// <param name="cosmosDbWrapper">the cosmos db wrapper</param>
        public LocalAuthorityStore(
            IWrapCosmosDbClient cosmosDbWrapper)
        {
            It.IsNull(cosmosDbWrapper)
                .AsGuard<ArgumentNullException>(nameof(cosmosDbWrapper));

            CosmosDbWrapper = cosmosDbWrapper;

        }

        /// <summary>
        /// get (the) local authority for...
        /// </summary>
        /// <param name="theAdminDistrict">the admin distict (code)</param>
        /// <returns>a local authority</returns>
        public async Task<ILocalAuthority> Get(string theAdminDistrict)
        {
            var localAuthority = await CosmosDbWrapper.GetLocalAuthorityAsync(theAdminDistrict, _partitionKey);

            It.IsNull(localAuthority)
                .AsGuard<NoContentException>(theAdminDistrict);

            return localAuthority;
        }

        /// <summary>
        /// add...
        /// </summary>
        /// <param name="theCandidate">the candidate (authority)</param>
        /// <returns>the newly added local authority</returns>
        public async Task<ILocalAuthority> Add(IncomingLocalAuthority theCandidate)
        {
            It.IsNull(theCandidate)
                .AsGuard<ArgumentNullException>(nameof(theCandidate));

            var theTouchpoint = theCandidate.TouchpointID;
            It.IsEmpty(theTouchpoint)
                .AsGuard<ArgumentNullException>(nameof(theTouchpoint));

            var theAdminDistrict = theCandidate.LADCode;
            It.IsNull(theAdminDistrict)
                .AsGuard<ArgumentNullException>(nameof(theAdminDistrict));

            (await CosmosDbWrapper.LocalAuthorityExistsAsync(theAdminDistrict, _partitionKey))
                .AsGuard<ConflictingResourceException>();

            var response = await CosmosDbWrapper.CreateLocalAuthorityAsync(theCandidate, _partitionKey);

            return response.Resource;
        }

        /// <summary>
        /// delete...
        /// </summary>
        /// <param name="theAdminDistrict">the admin distict (code)</param>
        /// <returns>the currently running task</returns>
        public async Task Delete(string theAdminDistrict)
        {
            It.IsEmpty(theAdminDistrict)
                .AsGuard<ArgumentNullException>(nameof(theAdminDistrict));

            (!await CosmosDbWrapper.LocalAuthorityExistsAsync(theAdminDistrict, _partitionKey))
                .AsGuard<NoContentException>();

            await CosmosDbWrapper.DeleteLocalAuthorityAsync(theAdminDistrict, _partitionKey);
        }
    }
}