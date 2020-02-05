using System;
using System.Threading.Tasks;

namespace DFC.FutureAccessModel.LocalAuthorities.Wrappers
{
    /// <summary>
    /// i document client (shim)
    /// </summary>
    public interface IWrapDocumentClient
    {
        /// <summary>
        /// create document (async)
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="documentCollectionUri">the document collection path</param>
        /// <param name="document">the document</param>
        /// <returns>the stored document</returns>
        Task<TResource> CreateDocumentAsync<TResource>(Uri documentCollectionUri, TResource document)
            where TResource : class;

        /// <summary>
        /// document exists (async)
        /// this will throw if the document does not exist
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="documentUri">the path to the document</param>
        /// <param name="partitionKey">the partition key</param>
        /// <returns>true if it does</returns>
        Task<bool> DocumentExistsAsync<TDocument>(Uri documentUri, string partitionKey)
            where TDocument : class;

        /// <summary>
        /// read document (async)
        /// throws if the document does not exist
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="documentUri">the doucment path</param>
        /// <param name="partitionKey">the partition key</param>
        /// <returns>an instance of the requested type <typeparamref name="TResource"/></returns>
        Task<TDocument> ReadDocumentAsync<TDocument>(Uri documentUri, string partitionKey)
            where TDocument : class;
    }
}
