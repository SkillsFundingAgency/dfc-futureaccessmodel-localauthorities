﻿using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Storage;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DFC.FutureAccessModel.LocalAuthorities.Wrappers.Internal
{
    /// <summary>
    /// the document client wrapper
    /// </summary>
    internal sealed class DocumentClientWrapper :
        IWrapDocumentClient
    {
        /// <summary>
        /// the document client
        /// </summary>
        public IDocumentClient Client { get; }

        /// <summary>
        /// initialises an instance of <see cref="DocumentClientWrapper"/>
        /// </summary>
        /// <param name="forEndpoint">for end point</param>
        /// <param name="usingAccountKey">using account key</param>
        [ExcludeFromCodeCoverage]
        public DocumentClientWrapper(Uri forEndpoint, string usingAccountKey) :
            this(new DocumentClient(forEndpoint, usingAccountKey))
        { }

        /// <summary>
        /// initialises an instance of <see cref="DocumentClientWrapper"/>
        /// using the principle of poor man's DI
        /// </summary>
        /// <param name="theClient">the client</param>
        public DocumentClientWrapper(IDocumentClient theClient)
        {
            It.IsNull(theClient)
                .AsGuard<ArgumentNullException>(nameof(theClient));

            Client = theClient;
        }

        /// <summary>
        /// create document (async)
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="usingCollectionPath">using collection path</param>
        /// <param name="andCandidate">the candidate (document)</param>
        /// <returns>the stored document</returns>
        public async Task<TDocument> CreateDocumentAsync<TDocument>(Uri usingCollectionPath, TDocument andCandidate)
            where TDocument : class
        {
            await Client.CreateDocumentAsync(usingCollectionPath, andCandidate);

            var documentUri = MakeDocumentPathFor(andCandidate, usingCollectionPath);
            var partitionKey = GetPartitionKey(andCandidate);

            return await ReadDocumentAsync<TDocument>(documentUri, partitionKey) ?? default;
        }

        /// <summary>
        /// document exists (async)
        /// this will throw if the document does not exist
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <param name="andPartitionKey">and partition key</param>
        /// <returns>true if it does</returns>
        public async Task<bool> DocumentExistsAsync<TDocument>(Uri usingStoragePath, string andPartitionKey)
            where TDocument : class =>
            It.Has(await ReadDocumentAsync<TDocument>(usingStoragePath, andPartitionKey));

        /// <summary>
        /// read document (async)
        /// throws if the document does not exist
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <param name="andPartitionKey">and partition key</param>
        /// <returns>an instance of the requested type <typeparamref name="TDocument"/></returns>
        public async Task<TDocument> ReadDocumentAsync<TDocument>(Uri usingStoragePath, string andPartitionKey)
            where TDocument : class
        {
            var response = await Client.ReadDocumentAsync<TDocument>(usingStoragePath, GetRequestOptions(andPartitionKey));
            return response?.Document;
        }

        /// <summary>
        /// delete document (async)
        /// throws if the document does not exist
        /// </summary>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <param name="andPartitionKey">and partition key</param>
        /// <returns>the running task</returns>
        public async Task DeleteDocumentAsync(Uri usingStoragePath, string andPartitionKey) =>
            await Client.DeleteDocumentAsync(usingStoragePath, GetRequestOptions(andPartitionKey));

        /// <summary>
        /// get (the) property details for...
        /// </summary>
        /// <typeparam name="TResource">the type of resource</typeparam>
        /// <typeparam name="TAttribute">the type of attribute</typeparam>
        /// <returns></returns>
        internal PropertyInfo GetPropertyDetails<TResource, TAttribute>(TResource theItem = null)
            where TResource : class
            where TAttribute : Attribute
        {
            var requiredType = theItem?.GetType() ?? typeof(TResource); // in case of it being interfaced

            return requiredType
                .GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute<TAttribute>() != null);
        }

        /// <summary>
        /// get's the partition key from the item
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="theItem"></param>
        /// <returns>the partition key</returns>
        internal string GetPartitionKey<TResource>(TResource theItem)
            where TResource : class
        {
            var keyValueName = GetPropertyDetails<TResource, PartitionKeyAttribute>(theItem);
            return $"{keyValueName.GetValue(theItem)}";
        }

        /// <summary>
        /// get request options
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <returns>the request options with the partition key info</returns>
        internal RequestOptions GetRequestOptions(string partitionKey) =>
        new RequestOptions
        {
            PartitionKey = new PartitionKey(partitionKey),
        };

        /// <summary>
        /// make resource path
        /// </summary>
        /// <typeparam name="TResource">the type of resource</typeparam>
        /// <param name="theResource">the resource</param>
        /// <param name="usingCollectionPath"></param>
        /// <returns>the document path</returns>
        internal Uri MakeDocumentPathFor<TResource>(TResource theResource, Uri usingCollectionPath)
            where TResource : class
        {
            var keyValueName = GetPropertyDetails<TResource, KeyAttribute>(theResource);

            It.IsNull(keyValueName)
                .AsGuard<ArgumentNullException>(nameof(keyValueName));

            return new Uri($"{usingCollectionPath.OriginalString}/docs/{keyValueName.GetValue(theResource)}", UriKind.Relative);
        }
    }
}
