using DFC.FutureAccessModel.LocalAuthorities.Factories;
using Microsoft.AspNetCore.Mvc;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers
{
    /// <summary>
    /// i provide fault responses
    /// </summary>
    public interface IProvideFaultResponses
    {
        /// <summary>
        /// get (the) response for...
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <param name="useLoggingScope">use (the) logging scope</param>
        /// <returns>the currently running task containing the http response message</returns>
        Task<IActionResult> GetResponseFor(Exception theException, IScopeLoggingContext useLoggingScope);
    }
}
