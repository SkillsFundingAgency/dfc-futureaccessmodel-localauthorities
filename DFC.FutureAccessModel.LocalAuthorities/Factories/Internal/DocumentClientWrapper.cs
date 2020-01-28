using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using System.Diagnostics.CodeAnalysis;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Microsoft.Azure.Documents.Client;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the document client wrapper
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class DocumentClientWrapper :
        IWrapDocumentClient
    {
        /// <summary>
        /// the (production build) document client
        /// </summary>
        private readonly IDocumentClient _client;

        /// <summary>
        /// initialises an instance of <see cref="StoreClient"/>
        /// </summary>
        /// <param name="forEndpoint">for end point</param>
        /// <param name="usingAccountKey">using account key</param>
        public DocumentClientWrapper(Uri forEndpoint, string usingAccountKey)
        {
            _client = new DocumentClient(forEndpoint, usingAccountKey);
        }

        /// <summary>
        /// create document (async)
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="documentCollectionUri">the document collection path</param>
        /// <param name="document">the document</param>
        /// <returns>the stored document</returns>
        public async Task<TDocument> CreateDocumentAsync<TDocument>(Uri documentCollectionUri, TDocument document)
            where TDocument : class
        {
            var response = await _client.CreateDocumentAsync(documentCollectionUri, document);
            return await response.Resource.ConvertTo<TDocument>() ?? default;
        }

        /// <summary>
        /// document exists (async)
        /// this will throw if the document does not exist
        /// </summary>
        /// <param name="documentUri">the path to the document</param>
        /// <returns>true if it does</returns>
        public async Task<bool> DocumentExistsAsync(Uri documentUri)
        {
            var response = await _client.ReadDocumentAsync(documentUri);
            return It.Has(response?.Resource);
        }

        /// <summary>
        /// read document (async)
        /// throws if the document does not exist
        /// </summary>
        /// <typeparam name="TResource">the type of resource</typeparam>
        /// <param name="documentUri">the doucment path</param>
        /// <returns>an instance of the requested type <typeparamref name="TResource"/></returns>
        public async Task<TDocument> ReadDocumentAsync<TDocument>(Uri documentUri)
            where TDocument : class
        {
            var response = await _client.ReadDocumentAsync<TDocument>(documentUri);
            return response.Document;
        }
    }
}
