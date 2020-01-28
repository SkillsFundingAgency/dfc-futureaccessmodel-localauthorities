using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;

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
        private readonly Dictionary<Type, Func<Exception, HttpResponseMessage>> _faultMap = new Dictionary<Type, Func<Exception, HttpResponseMessage>>();

        /// <summary>
        /// initialises an instance of <see cref="FaultResponseProvider"/>
        /// </summary>
        public FaultResponseProvider()
        {
            _faultMap.Add(typeof(ConflictingResourceException), Conflicted);
            _faultMap.Add(typeof(MalformedRequestException), Malformed);
            _faultMap.Add(typeof(NoContentException), NoContent);
            _faultMap.Add(typeof(UnprocessableEntityException), UnprocessableEntity);
            _faultMap.Add(typeof(AccessForbiddenException), Forbidden);
            _faultMap.Add(typeof(UnauthorizedException), Unauthorized);
        }

        /// <summary>
        /// get (the) response for...
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <param name="useLoggingScope">use (the) logging scope</param>
        /// <returns>the currently running task containing the http response message</returns>
        public async Task<HttpResponseMessage> GetResponseFor(Exception theException, IScopeLoggingContext useLoggingScope)
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
        internal HttpResponseMessage Malformed(Exception theException) =>
            new HttpResponseMessage(HttpStatusCode.BadRequest)
                .SetContent(string.Empty);

        /// <summary>
        /// the conflicted request action
        /// </summary>
        /// <param name="theException">the exception</param>
        /// <returns>a conflicted message</returns>
        internal HttpResponseMessage Conflicted(Exception theException) =>
            new HttpResponseMessage(HttpStatusCode.Conflict)
                .SetContent(string.Empty);

        /// <summary>
        /// the forbidden action
        /// </summary>
        /// <returns>a 'forbidden' message</returns>
        internal HttpResponseMessage Forbidden(Exception theException) =>
            new HttpResponseMessage(HttpStatusCode.Forbidden)
                .SetContent(theException.Message);

        /// <summary>
        /// the no content action
        /// </summary>
        /// <returns>a 'no content' message</returns>
        internal HttpResponseMessage NoContent(Exception theException) =>
            new HttpResponseMessage(HttpStatusCode.NoContent)
                .SetContent(theException.Message);

        /// <summary>
        /// the unprocessable entity action
        /// </summary>
        /// <returns>a 'unprocessable entity' message</returns>
        internal HttpResponseMessage UnprocessableEntity(Exception theException) =>
            new HttpResponseMessage(LocalHttpStatusCode.UnprocessableEntity.AsHttpStatusCode())
                .SetContent(theException.Message);

        /// <summary>
        /// the unauthorised action
        /// </summary>
        /// <returns>an 'unauthorised' message</returns>
        internal HttpResponseMessage Unauthorized(Exception theException) =>
            new HttpResponseMessage(HttpStatusCode.Unauthorized)
                .SetContent(string.Empty);

        /// <summary>
        /// the unknown error action
        /// </summary>
        /// <returns>an 'internal server error' message</returns>
        internal HttpResponseMessage UnknownError(Exception theException) =>
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
                .SetContent(theException?.Message ?? theException?.StackTrace ?? string.Empty);
    }
}
