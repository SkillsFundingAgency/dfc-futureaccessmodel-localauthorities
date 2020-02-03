using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories
{
    /// <summary>
    /// i scope the logging context
    /// </summary>
    public interface IScopeLoggingContext :
        IDisposable
    {
        /// <summary>
        /// (log) enter method
        /// </summary>
        /// <param name="theMethod">the method (name)</param>
        /// <returns>the currently running task</returns>
        Task EnterMethod([CallerMemberName] string theMethod = null);

        /// <summary>
        /// (log) exit method
        /// </summary>
        /// <param name="theMethod">the method (name)</param>
        /// <returns>the currently running task</returns>
        Task ExitMethod([CallerMemberName] string theMethod = null);

        /// <summary>
        /// (log) information
        /// </summary>
        /// <param name="theMessage">the message</param>
        /// <returns>the currently running task</returns>
        Task Information(string theMessage);

        /// <summary>
        /// (log) exception detail
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <returns>the currently running task</returns>
        Task ExceptionDetail(Exception theException);
    }
}
