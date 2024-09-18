using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    /// <summary>
    /// get local authority by lad code function
    /// </summary>
    public sealed class GetLocalAuthorityByLadCodeFunction :
        LocalAuthorityFunction
    {
        private readonly ILogger<GetLocalAuthorityByLadCodeFunction> _logger;

        /// <summary>
        /// initialises an instance of <see cref="GetLocalAuthorityByLadCodeFunction"/>
        /// </summary>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authority management) adapter</param>
        public GetLocalAuthorityByLadCodeFunction(ICreateLoggingContextScopes factory,
            IManageLocalAuthorities adapter,
            ILogger<GetLocalAuthorityByLadCodeFunction> logger) : base(factory, adapter)
        {
            _logger = logger;
        }

        /// <summary>
        /// get authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local authority district code</param>
        /// <param name="inScope">in (logging) scope</param>
        /// <returns></returns>
        internal async Task<IActionResult> GetAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope) =>
            await Adapter.GetAuthorityFor(theTouchpoint, theLADCode, inScope);

        /// <summary>
        /// run...
        /// </summary>
        /// <param name="request">the request</param>        
        /// <param name="touchpointID">(the) touchpoint id</param>
        /// <param name="ladCode">(the) local administrative district code</param>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authorities management) adapter</param>
        /// <returns>the http response to the operation</returns>
        [Function("GetLocalAuthorityByLADCode")]
        [ProducesResponseType(typeof(LocalAuthority), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = FunctionDescription.ResourceFound, ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = FunctionDescription.NoContent, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = FunctionDescription.MalformedRequest, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = FunctionDescription.Unauthorised, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = FunctionDescription.Forbidden, ShowSchema = false)]
        [Display(Name = "Get Local Authority by Local Administrative District Code", Description = "Ability to get a Local Authority detail for the given Touchpoint and LADCode.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "areas/{touchpointID}/localauthorities/{ladCode}")] HttpRequest request,
            string touchpointID,
            string ladCode) =>
                await RunActionScope(request, _logger, x => GetAuthorityFor(touchpointID, ladCode, x));
    }
}