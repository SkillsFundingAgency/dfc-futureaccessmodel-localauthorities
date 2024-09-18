using DFC.FutureAccessModel.LocalAuthorities.Factories;
using Microsoft.AspNetCore.Mvc;

namespace DFC.FutureAccessModel.LocalAuthorities.Adapters
{
    /// <summary>
    /// i manage local authorities
    /// </summary>
    public interface IManageLocalAuthorities
    {
        /// <summary>
        /// get (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        Task<IActionResult> GetAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope);

        /// <summary>
        /// add new authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="usingContent">using content</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        Task<IActionResult> AddNewAuthorityFor(string theTouchpoint, string usingContent, IScopeLoggingContext inScope);

        /// <summary>
        /// delete (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        Task<IActionResult> DeleteAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope);
    }
}
