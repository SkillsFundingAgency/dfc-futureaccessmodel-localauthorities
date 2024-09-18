using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    /// <summary>
    /// the api definition for 'swagger' document generation 
    /// </summary>
    public sealed class ApiDefinitionFunction
    {
        public const string ApiTitle = "Admin District support for the Omni Channel Area Routing API";
        public const string ApiVersion = "1.0.0";
        public const string ApiDefinitionName = "api-definition";
        public const string ApiDescription =
            @"Management API for Local Authorities within Area Routing
            (enquiry redirection to local area contractors)";

        /// <summary>
        /// the (swagger document) generator
        /// </summary>
        public ISwaggerDocumentGenerator Generator { get; }

        /// <summary>
        /// initialises an instance of <see cref="ApiDefinitionFunction"/>
        /// </summary>
        /// <param name="generator">the (swagger document) generator</param>
        public ApiDefinitionFunction(ISwaggerDocumentGenerator generator)
        {
            It.IsNull(generator)
                .AsGuard<ArgumentNullException>(nameof(generator));

            Generator = generator;
        }

        /// <summary>
        /// run... (the api document generator function)
        /// </summary>
        /// <param name="request">the http request</param>
        /// <param name="theDocumentGenerator">the document generator</param>
        /// <returns>a http response containing the generated document</returns>
        [Function("ApiDefinition")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "localauthorities/api-definition")]HttpRequest request) =>
                await Task.Run<IActionResult>(() =>
                {
                    It.IsNull(request)
                        .AsGuard<ArgumentNullException>(nameof(request));

                    var document = Generator.GenerateSwaggerDocument(
                        request,
                        ApiTitle,
                        ApiDescription,
                        ApiDefinitionName,
                        ApiVersion,
                        Assembly.GetExecutingAssembly(),
                        false, false); // don't include some irrelevant default parameters

                    if (string.IsNullOrEmpty(document))
                        return new NoContentResult();

                    return new OkObjectResult(document);
                });
    }
}