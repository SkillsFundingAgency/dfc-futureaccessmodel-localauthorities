using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using System.Diagnostics.CodeAnalysis;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

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
        /// <typeparam name="TResource"></typeparam>
        /// <param name="documentCollectionUri">the document collection path</param>
        /// <param name="document">the document</param>
        /// <returns>the stored document</returns>
        public async Task<TDocument> CreateDocumentAsync<TDocument>(Uri documentCollectionUri, TDocument document)
            where TDocument : class
        {
            await Client.CreateDocumentAsync(documentCollectionUri, document);

            var documentUri = MakeDocumentPathFor(document, documentCollectionUri);
            return await ReadDocumentAsync<TDocument>(documentUri) ?? default;
        }

        /// <summary>
        /// document exists (async)
        /// this will throw if the document does not exist
        /// </summary>
        /// <param name="documentUri">the path to the document</param>
        /// <returns>true if it does</returns>
        public async Task<bool> DocumentExistsAsync(Uri documentUri)
        {
            var response = await Client.ReadDocumentAsync(documentUri);
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
            var response = await Client.ReadDocumentAsync<TDocument>(documentUri);
            return response.Document;
        }

        /// <summary>
        /// make resource path
        /// </summary>
        /// <typeparam name="TResource">the type of resource</typeparam>
        /// <param name="theResource">the resource</param>
        /// <param name="usingCollectionPath"></param>
        /// <returns></returns>
        internal Uri MakeDocumentPathFor<TResource>(TResource theResource, Uri usingCollectionPath)
        {
            var keyValueName = theResource.GetType()
                .GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null);

            It.IsNull(keyValueName)
                .AsGuard<ArgumentNullException>(nameof(keyValueName));

            return new Uri($"{usingCollectionPath.OriginalString}/docs/{keyValueName.GetValue(theResource)}", UriKind.Relative);
        }
    }
}
