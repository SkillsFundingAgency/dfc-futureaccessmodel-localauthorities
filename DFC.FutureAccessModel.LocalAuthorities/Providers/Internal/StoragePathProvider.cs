﻿using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Microsoft.Azure.Documents.Client;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the storage path provider
    /// </summary>
    internal sealed class StoragePathProvider :
        IProvideStoragePaths
    {
        /// <summary>
        /// the document store id (variable) key
        /// </summary>
        public const string DocumentStoreIDKey = "DocumentStoreID";

        /// <summary>
        /// the loca lauthority collection (variable) key
        /// </summary>
        public const string LocalAuthorityCollectionIDKey = "LocalAuthorityCollectionID";

        /// <summary>
        /// the document store id (variable value)
        /// </summary>
        public string DocumentStoreID { get; }

        /// <summary>
        /// the routing detail collection id (variable value)
        /// </summary>
        public string RoutingDetailCollectionID { get; }

        /// <summary>
        /// the local authority collection id (variable value)
        /// </summary>
        public string LocalAuthorityCollectionID { get; }

        /// <summary>
        /// the local authority collection (path), used for new documents
        /// </summary>
        public Uri LocalAuthorityCollection { get; }

        /// <summary>
        /// the applicaton settings provider
        /// </summary>
        public IProvideApplicationSettings Settings { get; }

        /// <summary>
        /// initialises the storage path provider
        /// </summary>
        public StoragePathProvider(IProvideApplicationSettings settings)
        {
            It.IsNull(settings)
                .AsGuard<ArgumentNullException>(nameof(settings));

            Settings = settings;

            DocumentStoreID = Settings.GetVariable(DocumentStoreIDKey);
            LocalAuthorityCollectionID = Settings.GetVariable(LocalAuthorityCollectionIDKey);

            LocalAuthorityCollection = UriFactory.CreateDocumentCollectionUri(DocumentStoreID, LocalAuthorityCollectionID);
        }

        /// <summary>
        /// get (the) local authority resource path for...
        /// </summary>
        /// <param name="theAdminDistrict">the admin district</param>
        /// <returns>the uri for the requested storage path</returns>
        public Uri GetLocalAuthorityResourcePathFor(string theAdminDistrict) =>
            UriFactory.CreateDocumentUri(DocumentStoreID, LocalAuthorityCollectionID, $"{theAdminDistrict}");
    }
}
