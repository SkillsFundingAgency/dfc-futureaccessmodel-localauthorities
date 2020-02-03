using System;
using System.Net;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using DFC.FutureAccessModel.LocalAuthorities.Wrappers;
using Microsoft.Azure.Documents;

namespace DFC.FutureAccessModel.LocalAuthorities.Storage.Internal
{
    /// <summary>
    ///  the document store
    /// </summary>
    internal sealed class DocumentStore :
        IStoreDocuments
    {
        /// <summary>
        /// the document store endpoint address key
        /// </summary>
        public const string DocumentStoreEndpointAddressKey = "DocumentStoreEndpointAddress";

        /// <summary>
        /// the document store account key
        /// </summary>
        public const string DocumentStoreAccountKey = "DocumentStoreAccountKey";

        /// <summary>
        /// the endpoint address
        /// </summary>
        public string EndpointAddress { get; }

        /// <summary>
        /// the 'azure' account key
        /// </summary>
        public string AccountKey { get; }

        /// <summary>
        /// the document store client
        /// </summary>
        public IWrapDocumentClient Client { get; }

        /// <summary>
        /// the safe operations (provider)
        /// </summary>
        public IProvideSafeOperations SafeOperations { get; }

        /// <summary>
        /// initialises an instance of the <see cref="DocumentStore"/>
        /// </summary>
        /// <param name="usingEnvironment">using environment variables</param>
        public DocumentStore(
            IProvideApplicationSettings usingEnvironment,
            ICreateDocumentClients factory,
            IProvideSafeOperations safeOperations)
        {
            It.IsNull(usingEnvironment)
                .AsGuard<ArgumentNullException>(nameof(usingEnvironment));
            It.IsNull(factory)
                .AsGuard<ArgumentNullException>(nameof(factory));
            It.IsNull(safeOperations)
                .AsGuard<ArgumentNullException>(nameof(safeOperations));

            EndpointAddress = usingEnvironment.GetVariable(DocumentStoreEndpointAddressKey);
            It.IsEmpty(EndpointAddress)
                .AsGuard<ArgumentNullException>(nameof(EndpointAddress));

            AccountKey = usingEnvironment.GetVariable(DocumentStoreAccountKey);
            It.IsEmpty(AccountKey)
                .AsGuard<ArgumentNullException>(nameof(AccountKey));

            Client = factory.CreateClient(new Uri(EndpointAddress), AccountKey);
            SafeOperations = safeOperations;
        }

        /// <summary>
        /// document exists
        /// </summary>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <returns>true if the document exists</returns>
        public async Task<bool> DocumentExists(Uri usingStoragePath) =>
            await SafeOperations.Try(
                () => ProcessDocumentExists(usingStoragePath),
                x => Task.FromResult(false));

        /// <summary>
        /// document exists
        /// </summary>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <returns>true if the document exists</returns>
        internal async Task<bool> ProcessDocumentExists(Uri usingStoragePath) =>
            await Client.DocumentExistsAsync(usingStoragePath);

        /// <summary>
        /// add (a) document (to the document store)
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="theDocument">the document</param>
        /// <param name="usingCollectionPath">using (the) collection path</param>
        /// <returns>the currently running task</returns>
        public async Task<TDocument> AddDocument<TDocument>(TDocument theDocument, Uri usingCollectionPath)
            where TDocument : class =>
            await SafeOperations.Try(
                () => ProcessAddDocument(usingCollectionPath, theDocument),
                x => ProcessDocumentErrorHandler<TDocument>(x));

        /// <summary>
        /// process add document
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="usingCollectionPath">using (the) collection path</param>
        /// <param name="theDocument">the document</param>
        /// <returns>the currently running task</returns>
        internal async Task<TDocument> ProcessAddDocument<TDocument>(Uri usingCollectionPath, TDocument theDocument)
            where TDocument : class =>
            await Client.CreateDocumentAsync(usingCollectionPath, theDocument);

        /// <summary>
        /// get (a) document (from the document store)
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <returns>the currently running task</returns>
        public async Task<TDocument> GetDocument<TDocument>(Uri usingStoragePath)
            where TDocument : class =>
            await SafeOperations.Try(
                () => ProcessGetDocument<TDocument>(usingStoragePath),
                x => ProcessDocumentErrorHandler<TDocument>(x));

        /// <summary>
        /// process get document
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <returns></returns>
        internal async Task<TDocument> ProcessGetDocument<TDocument>(Uri usingStoragePath)
            where TDocument : class =>
            await Client.ReadDocumentAsync<TDocument>(usingStoragePath);

        /// <summary>
        /// process get document error handler. 
        /// safe handling and exception transformation into something the API can deal with. 
        /// an incoming null is likely to be the result of an argument null 
        /// exception for an 'invalid' uri in the read document call. 
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="theException">the exception</param>
        /// <returns>nothing, the expectation is to throw an 'application' exception</returns>
        internal async Task<TDocument> ProcessDocumentErrorHandler<TDocument>(Exception theException)
            where TDocument : class =>
            await Task.Run(() =>
            {
                ProcessError(theException as DocumentClientException);

                // we don't expect to ever get here...
                return default(TDocument);
            });

        /// <summary>
        /// process (the) error
        /// </summary>
        /// <param name="theException">the exception</param>
        internal void ProcessError(DocumentClientException theException)
        {
            It.IsNull(theException)
                .AsGuard<MalformedRequestException>();

            (HttpStatusCode.NotFound == theException.StatusCode)
                .AsGuard<NoContentException>();

            LocalHttpStatusCode.TooManyRequests.ComparesTo(theException.StatusCode)
                .AsGuard<MalformedRequestException>();
        }
    }
}
