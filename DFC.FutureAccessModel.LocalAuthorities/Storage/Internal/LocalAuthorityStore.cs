using System;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Providers;

namespace DFC.FutureAccessModel.LocalAuthorities.Storage.Internal
{
    /// <summary>
    /// the area routing detail store
    /// </summary>
    internal sealed class LocalAuthorityStore :
        IStoreLocalAuthorities
    {
        /// <summary>
        /// storage paths
        /// </summary>
        public IProvideStoragePaths StoragePaths { get; }

        /// <summary>
        /// the (underlying) document store
        /// </summary>
        public IStoreDocuments DocumentStore { get; }

        /// <summary>
        /// create an instance of hte <see cref="LocalAuthorityStore"/>
        /// </summary>
        /// <param name="paths">the storage paths (provider)</param>
        /// <param name="store">the document store</param>
        public LocalAuthorityStore(
            IProvideStoragePaths paths,
            IStoreDocuments store)
        {
            It.IsNull(paths)
                .AsGuard<ArgumentNullException>(nameof(paths));
            It.IsNull(store)
                .AsGuard<ArgumentNullException>(nameof(store));

            StoragePaths = paths;
            DocumentStore = store;
        }

        /// <summary>
        /// get (the) local authority for...
        /// </summary>
        /// <param name="theAdminDistrict">the admin distict (code)</param>
        /// <returns>a local authority</returns>
        public async Task<ILocalAuthority> Get(string theAdminDistrict)
        {
            var usingPath = StoragePaths.GetLocalAuthorityResourcePathFor(theAdminDistrict);
            return await DocumentStore.GetDocument<LocalAuthority>(usingPath);
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="theCandidate">the candidate (authority)</param>
        /// <returns>the newly added local authority</returns>
        public async Task<ILocalAuthority> Add(ILocalAuthority theCandidate)
        {
            It.IsNull(theCandidate)
                .AsGuard<ArgumentNullException>(nameof(theCandidate));

            var theTouchpoint = theCandidate.TouchpointID;
            It.IsNull(theTouchpoint)
                .AsGuard<ArgumentNullException>(nameof(theTouchpoint));

            var usingAreaPath = StoragePaths.GetRoutingDetailResourcePathFor(theTouchpoint);

            (!await DocumentStore.DocumentExists(usingAreaPath))
                .AsGuard<NoContentException>(theTouchpoint);

            var theAdminDistrict = theCandidate.LADCode;
            It.IsNull(theAdminDistrict)
                .AsGuard<ArgumentNullException>(nameof(theAdminDistrict));

            var usingAuthorityPath = StoragePaths.GetLocalAuthorityResourcePathFor(theAdminDistrict);

            (await DocumentStore.DocumentExists(usingAuthorityPath))
                .AsGuard<ConflictingResourceException>();

            return await DocumentStore.AddDocument(theCandidate, StoragePaths.LocalAuthorityCollection);
        }
    }
}