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
        /// document exists
        /// </summary>
        /// <param name="usingStoragePath">using (the) storage path</param>
        /// <returns>true if the document exists</returns>
        Task<bool> DocumentExists(Uri usingStoragePath);

        /// <summary>
        /// add (a) document (to the document store)
        /// </summary>
        /// <typeparam name="TDocument">the document type</typeparam>
        /// <param name="theDocument">the document</param>
        /// <param name="usingCollectionPath">using (the) collection path</param>
        /// <returns>the currently running task</returns>
        Task<TDocument> AddDocument<TDocument>(TDocument theDocument, Uri usingCollectionPath)
            where TDocument : class;

        /// <summary>
        /// get document
        /// </summary>
        /// <typeparam name="TDocument">of this type</typeparam>
        /// <param name="usingStoragePath">using the storage path</param>
        /// <returns></returns>
        Task<TDocument> GetDocument<TDocument>(Uri usingStoragePath)
            where TDocument : class;
    }
}