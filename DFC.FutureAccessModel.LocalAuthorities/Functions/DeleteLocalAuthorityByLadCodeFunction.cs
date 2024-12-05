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
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    public sealed class DeleteLocalAuthorityByLadCodeFunction :
        LocalAuthorityFunction
    {
        private readonly ILogger<DeleteLocalAuthorityByLadCodeFunction> _logger;

        /// <summary>
        /// initialises an instance of <see cref="DeleteLocalAuthorityByLadCodeFunction"/>
        /// </summary>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authority management) adapter</param>
        public DeleteLocalAuthorityByLadCodeFunction(
            ICreateLoggingContextScopes factory,
            IManageLocalAuthorities adapter,
            ILogger<DeleteLocalAuthorityByLadCodeFunction> logger) : base(factory, adapter)
        {
            _logger = logger;
        }

        /// <summary>
        /// delete authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local authority district code</param>
        /// <param name="inScope">in (logging) scope</param>
        /// <returns></returns>
        internal async Task<IActionResult> DeleteAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope) =>
            await Adapter.DeleteAuthorityFor(theTouchpoint, theLADCode, inScope);

        /// <summary>
        /// run...
        /// </summary>
        /// <param name="theRequest">the request</param>        
        /// <param name="touchpointID">(the) touchpoint id</param>
        /// <param name="ladCode">(the) local administrative district code</param>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authorities management) adapter</param>
        /// <returns>the http response to the operation</returns>
        [Function("DeleteLocalAuthorityByLADCode")]
        [ProducesResponseType(typeof(LocalAuthority), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = FunctionDescription.ResourceFound, ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = FunctionDescription.NoContent, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = FunctionDescription.Forbidden, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = FunctionDescription.MalformedRequest, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = FunctionDescription.Unauthorised, ShowSchema = false)]
        [Display(Name = "Delete a Local Authority by Local Administrative District Code", Description = "Ability to get a Local Authority detail for the given Touchpoint and LADCode.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "areas/{touchpointID}/localauthorities/{ladCode}")] HttpRequest theRequest,
            string touchpointID,
            string ladCode) =>
                await RunActionScope(theRequest, _logger, x => DeleteAuthorityFor(touchpointID, ladCode, x));
    }
}