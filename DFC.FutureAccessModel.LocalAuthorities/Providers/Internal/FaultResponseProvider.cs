using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web.Http;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the fault response provider
    /// </summary>
    internal sealed class FaultResponseProvider :
        IProvideFaultResponses
    {
        /// <summary>
        /// the method action map
        /// </summary>
        private readonly Dictionary<Type, Func<Exception, IActionResult>> _faultMap = new Dictionary<Type, Func<Exception, IActionResult>>();

        /// <summary>
        /// initialises an instance of <see cref="FaultResponseProvider"/>
        /// </summary>
        public FaultResponseProvider()
        {
            _faultMap.Add(typeof(ConflictingResourceException), Conflicted);
            _faultMap.Add(typeof(MalformedRequestException), Malformed);
            _faultMap.Add(typeof(NoContentException), NoContent);
            _faultMap.Add(typeof(UnprocessableEntityException), UnprocessableEntity);
        }

        /// <summary>
        /// get (the) response for...
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <param name="useLoggingScope">use (the) logging scope</param>
        /// <returns>the currently running task containing the http response message</returns>
        public async Task<IActionResult> GetResponseFor(Exception theException, IScopeLoggingContext useLoggingScope)
        {
            if (_faultMap.ContainsKey(theException.GetType()))
            {
                await InformOn(theException, useLoggingScope);
                return _faultMap[theException.GetType()].Invoke(theException);
            }

            await useLoggingScope.ExceptionDetail(theException);

            return UnknownError(theException);
        }

        /// <summary>
        /// infomr on...
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <param name="useLoggingScope">using (the) logging scope</param>
        /// <returns></returns>
        internal async Task InformOn(Exception theException, IScopeLoggingContext useLoggingScope)
        {
            await useLoggingScope.Information(theException.Message);

            if (It.Has(theException.InnerException))
            {
                await InformOn(theException.InnerException, useLoggingScope);
            }
        }

        /// <summary>
        /// the malformed request action
        /// </summary>
        /// <returns>a 'bad request' message</returns>
        internal static IActionResult Malformed(Exception theException) =>
            new BadRequestObjectResult(HttpStatusCode.BadRequest);

        /// <summary>
        /// the conflicted request action
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <returns>a conflicted message</returns>
        internal static IActionResult Conflicted(Exception theException) =>
            new ConflictObjectResult(HttpStatusCode.Conflict);

        /// <summary>
        /// the no content action
        /// </summary>
        /// <returns>a 'no content' message</returns>
        internal static IActionResult NoContent(Exception theException) =>
            new NoContentResult();

        /// <summary>
        /// the unprocessable entity action
        /// </summary>
        /// <returns>a 'unprocessable entity' message</returns>
        internal static IActionResult UnprocessableEntity(Exception theException) =>
            new UnprocessableEntityObjectResult(theException.Message);

        /// <summary>
        /// the unknown error action
        /// </summary>
        /// <returns>an 'internal server error' message</returns>
        internal static IActionResult UnknownError(Exception theException) =>
            new InternalServerErrorResult();
    }
}
