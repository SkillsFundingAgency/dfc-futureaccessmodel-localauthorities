using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Registration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories
{
    /// <summary>
    /// i create logging context scopes
    /// </summary>
    public interface ICreateLoggingContextScopes :
        ISupportServiceRegistration
    {
        /// <summary>
        /// begin scope
        /// </summary>
        /// <param name="theRequest">the request</param>
        /// <param name="usingTraceWriter">using (the) trace writer</param>
        /// <param name="initialisingRoutine">initalising routine</param>
        /// <returns>a new logging scope</returns>
        Task<IScopeLoggingContext> BeginScopeFor(
            HttpRequest theRequest,
            ILogger usingTraceWriter,
            [CallerMemberName] string initialisingRoutine = null);
    }
}
