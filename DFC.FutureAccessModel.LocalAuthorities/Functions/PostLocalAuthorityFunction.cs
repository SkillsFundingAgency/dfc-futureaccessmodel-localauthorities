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
    /// post local authority function
    /// </summary>
    public sealed class PostLocalAuthorityFunction :
        LocalAuthorityFunction
    {
        /// <summary>
        /// initialises an instance of <see cref="PostLocalAuthorityFunction"/>
        /// </summary>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authority management) adapter</param>
        public PostLocalAuthorityFunction(ICreateLoggingContextScopes factory, IManageLocalAuthorities adapter) : base(factory, adapter) { }

        /// <summary>
        /// add new authority using...
        /// </summary>
        /// <param name="theRequest">the request</param>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="inScope">in (logging) scope</param>
        /// <returns></returns>
        internal async Task<HttpResponseMessage> AddNewAuthorityUsing(HttpRequest theRequest, string theTouchpoint, IScopeLoggingContext inScope)
        {
            var theContent = await theRequest.ReadAsStringAsync();
            return await Adapter.AddNewAuthorityFor(theTouchpoint, theContent, inScope);
        }

        /// <summary>
        /// run...
        /// </summary>
        /// <param name="theRequest">the request</param>
        /// <param name="usingTraceWriter">using (the) trace writer</param>
        /// <param name="touchpointID">(the) touchpoint id</param>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authorities management) adapter</param>
        /// <returns>the http response to the operation</returns>
        [FunctionName("PostLocalAuthority")]
        [ProducesResponseType(typeof(LocalAuthority), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = FunctionDescription.ResourceCreated, ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = FunctionDescription.NoParentContent, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = FunctionDescription.MalformedRequest, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Conflict, Description = FunctionDescription.Conflict, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = FunctionDescription.Unauthorised, ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = FunctionDescription.Forbidden, ShowSchema = false)]
        [Display(Name = "Post the details of a new Local Authority", Description = "Ability to add the Local Authority details for the given Touchpoint.")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "areas/{touchpointID}/localauthorities")]HttpRequest theRequest,
            ILogger usingTraceWriter,
            string touchpointID) =>
                await RunActionScope(theRequest, usingTraceWriter, x => AddNewAuthorityUsing(theRequest, touchpointID, x));
    }
}