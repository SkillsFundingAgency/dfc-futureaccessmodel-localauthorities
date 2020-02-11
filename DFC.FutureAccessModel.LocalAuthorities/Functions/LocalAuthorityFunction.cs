using System;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    public abstract class LocalAuthorityFunction
    {
        /// <summary>
        /// (the logging scope) factory
        /// </summary>
        public ICreateLoggingContextScopes Factory { get; }

        /// <summary>
        /// initialises an instance of <see cref="LocalAuthorityFunction"/>
        /// </summary>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authority management) adapter</param>
        protected LocalAuthorityFunction(ICreateLoggingContextScopes factory)
        {
            It.IsNull(factory)
                .AsGuard<ArgumentNullException>(nameof(factory));

            Factory = factory;
        }

        /// <summary>
        /// run action scope...
        /// </summary>
        /// <param name="theRequest">the request</param>
        /// <param name="usingTraceWriter">using (the) trace writer</param>
        /// <param name="actionDo">(the) action (to) do</param>
        /// <returns>the http response to the operation</returns>
        public async Task<HttpResponseMessage> RunActionScope(
            HttpRequest theRequest,
            ILogger usingTraceWriter,
            Func<IScopeLoggingContext, Task<HttpResponseMessage>> actionDo)
        {
            It.IsNull(theRequest)
                .AsGuard<ArgumentNullException>(nameof(theRequest));
            It.IsNull(usingTraceWriter)
                .AsGuard<ArgumentNullException>(nameof(usingTraceWriter));

            using (var inScope = await Factory.BeginScopeFor(theRequest, usingTraceWriter))
            {
                return await actionDo(inScope);
            }
        }
    }
}