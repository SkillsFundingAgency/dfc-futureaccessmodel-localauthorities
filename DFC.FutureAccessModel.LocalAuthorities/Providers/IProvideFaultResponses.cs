using System;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Registration;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers
{
    /// <summary>
    /// i provide fault responses
    /// </summary>
    public interface IProvideFaultResponses :
        ISupportServiceRegistration
    {
        /// <summary>
        /// get (the) response for...
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <param name="useLoggingScope">use (the) logging scope</param>
        /// <returns>the currently running task containing the http response message</returns>
        Task<HttpResponseMessage> GetResponseFor(Exception theException, IScopeLoggingContext useLoggingScope);
    }
}
