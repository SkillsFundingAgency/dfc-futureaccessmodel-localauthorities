using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DFC.Common.Standard.Logging;
using Microsoft.Extensions.Logging;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the logging scope
    /// </summary>
    internal sealed class LoggingContextScope :
        IScopeLoggingContext
    {
        /// <summary>
        /// the microsoft logger
        /// </summary>
        public ILogger Log { get; }

        /// <summary>
        /// the DFC standard logger helper
        /// </summary>
        public ILoggerHelper LoggerHelper { get; }

        /// <summary>
        /// a 'correlation' id
        /// not sure of the benefit here, but it seems to be needed for some calls
        /// </summary>
        public Guid CorrelationID { get; }

        /// <summary>
        /// initialises an instance of <see cref="LoggingContextScope"/>
        /// </summary>
        /// <param name="log">the microsoft log</param>
        /// <param name="helper">the DFC logger helper</param>
        /// <param name="initialisingRoutine">the calling routine</param>
        public LoggingContextScope(ILogger log, ILoggerHelper helper, string initialisingRoutine)
        {
            Log = log;
            LoggerHelper = helper;

            CorrelationID = Guid.NewGuid();

            Information($"commencing scoped logging from: '{initialisingRoutine}' for correlation id: {CorrelationID}").ConfigureAwait(false);
        }

        /// <summary>
        /// (log) enter method
        /// </summary>
        /// <param name="theMethod">the method (name)</param>
        /// <returns>the currently running task</returns>
        public async Task EnterMethod([CallerMemberName] string theMethod = null) =>
            await Information($"entering method: '{theMethod}'");

        /// <summary>
        /// (log) exit method
        /// </summary>
        /// <param name="theMethod">the method (name)</param>
        /// <returns>the currently running task</returns>
        public async Task ExitMethod([CallerMemberName] string theMethod = null) =>
            await Information($"exiting method: '{theMethod}'");

        /// <summary>
        /// (log) information
        /// </summary>
        /// <param name="theMessage">the message</param>
        /// <returns>the currently running task</returns>
        public async Task Information(string theMessage) =>
            await Task.Run(() => LoggerHelper.LogInformationMessage(Log, CorrelationID, theMessage));

        /// <summary>
        /// (log) exception detail
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <returns>the currently running task</returns>
        public async Task ExceptionDetail(Exception theException) =>
            await Task.Run(() => LoggerHelper.LogException(Log, CorrelationID, theException));

        /// <summary>
        /// dispose, as this is used in a 'using' clause disposal should be guaranteed
        /// </summary>
        public void Dispose()
        {
            LoggerHelper.LogInformationMessage(Log, CorrelationID, "request completed");
        }
    }
}
