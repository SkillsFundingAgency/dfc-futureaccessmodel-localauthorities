using System;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Registration;

namespace DFC.FutureAccessModel.LocalAuthorities.Storage
{
    /// <summary>
    /// i store documents
    /// </summary>
    public interface IStoreDocuments :
        ISupportServiceRegistration
    {
        /// <summary>
        /// document exists...
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <param name="partitionKey">the partition key</param>
        /// <returns>true if the document exists</returns>
        Task<bool> DocumentExists<TDocument>(Uri usingStoragePath, string partitionKey)
            where TDocument : class;

        /// <summary>
        /// add (a) document (to the document store)
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="theDocument">the document</param>
        /// <param name="usingCollectionPath">using (the) collection path</param>
        /// <returns>the submitted document</returns>
        Task<TDocument> AddDocument<TDocument>(TDocument theDocument, Uri usingCollectionPath)
            where TDocument : class;

        /// <summary>
        /// get document
        /// </summary>
        /// <typeparam name="TDocument">of this type</typeparam>
        /// <param name="usingStoragePath">using the storage path</param>
        /// <param name="partitionKey">the partition key</param>
        /// <returns>the requested document</returns>
        Task<TDocument> GetDocument<TDocument>(Uri usingStoragePath, string partitionKey)
            where TDocument : class;
    }
}