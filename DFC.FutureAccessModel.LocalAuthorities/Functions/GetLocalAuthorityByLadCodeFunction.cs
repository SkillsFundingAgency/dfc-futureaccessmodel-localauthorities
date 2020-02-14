using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    /// <summary>
    /// get local authority by lad code function
    /// </summary>
    public sealed class GetLocalAuthorityByLadCodeFunction :
        LocalAuthorityFunction
    {
        /// <summary>
        /// initialises an instance of <see cref="GetLocalAuthorityByLadCodeFunction"/>
        /// </summary>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authority management) adapter</param>
        public GetLocalAuthorityByLadCodeFunction(ICreateLoggingContextScopes factory, IManageLocalAuthorities adapter) : base(factory, adapter) { }

        /// <summary>
        /// get authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local authority district code</param>
        /// <param name="inScope">in (logging) scope</param>
        /// <returns></returns>
        internal async Task<HttpResponseMessage> GetAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope) =>
            await Adapter.GetAuthorityFor(theTouchpoint, theLADCode, inScope);

        /// <summary>
        /// run...
        /// </summary>
        /// <param name="theRequest">the request</param>
        /// <param name="usingTraceWriter">using (the) trace writer</param>
        /// <param name="touchpointID">(the) touchpoint id</param>
        /// <param name="ladCode">(the) local administrative district code</param>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authorities management) adapter</param>
        /// <returns>the http response to the operation</returns>
        [FunctionName("GetLocalAuthorityByLADCode")]
        [ProducesResponseType(typeof(LocalAuthority), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = FunctionDescription.ResourceFound, ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = FunctionDescription.NoContent, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = FunctionDescription.MalformedRequest, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = FunctionDescription.Unauthorised, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = FunctionDescription.Forbidden, ShowSchema = false)]
        [Display(Name = "Get Local Authority by Local Administrative District Code", Description = "Ability to get a Local Authority detail for the given Touchpoint and LADCode.")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", Route = "areas/{touchpointID}/localauthorities/{ladCode}")]HttpRequest theRequest,
            ILogger usingTraceWriter,
            string touchpointID,
            string ladCode) =>
                await RunActionScope(theRequest, usingTraceWriter, x => GetAuthorityFor(touchpointID, ladCode, x));
    }
}