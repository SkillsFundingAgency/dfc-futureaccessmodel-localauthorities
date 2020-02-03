using System;
using DFC.FutureAccessModel.LocalAuthorities.Registration;
using DFC.FutureAccessModel.LocalAuthorities.Wrappers;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories
{
    /// <summary>
    /// i create document clients
    /// </summary>
    public interface ICreateDocumentClients :
        ISupportServiceRegistration
    {
        /// <summary>
        /// create client...
        /// </summary>
        /// <param name="forEndpoint">for endpoint</param>
        /// <param name="usingAccountKey">using account key</param>
        /// <returns>a newly constructed document client</returns>
        IWrapDocumentClient CreateClient(Uri forEndpoint, string usingAccountKey);
    }
}
