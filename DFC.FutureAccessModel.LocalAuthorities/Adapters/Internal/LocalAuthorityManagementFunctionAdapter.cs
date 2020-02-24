using System;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using DFC.FutureAccessModel.LocalAuthorities.Storage;
using DFC.FutureAccessModel.LocalAuthorities.Validation;
using DFC.HTTP.Standard;
using Newtonsoft.Json;

namespace DFC.FutureAccessModel.LocalAuthorities.Adapters.Internal
{
    /// <summary>
    /// the local authorities management function adapter
    /// </summary>
    internal sealed class LocalAuthorityManagementFunctionAdapter :
        IManageLocalAuthorities
    {
        /// <summary>
        /// the fault (response provider)
        /// </summary>
        public IProvideFaultResponses Faults { get; }

        /// <summary>
        /// the safe operations (provider)
        /// </summary>
        public IProvideSafeOperations SafeOperations { get; }

        /// <summary>
        /// the response (helper)
        /// </summary>
        public IHttpResponseMessageHelper Respond { get; }

        /// <summary>
        /// i store local authorities
        /// </summary>
        public IStoreLocalAuthorities Authorities { get; }

        /// <summary>
        /// i validate local authorities
        /// </summary>
        public IValidateLocalAuthorities Authority { get; }

        /// <summary>
        /// instantiates a new instance of <see cref="LocalAuthorityManagementFunctionAdapter"/>
        /// </summary>
        /// <param name="responseHelper"></param>
        /// <param name="faultResponses"></param>
        /// <param name="safeOperations"></param>
        /// <param name="authorities"></param>
        public LocalAuthorityManagementFunctionAdapter(
            IHttpResponseMessageHelper responseHelper,
            IProvideFaultResponses faultResponses,
            IProvideSafeOperations safeOperations,
            IStoreLocalAuthorities authorities,
            IValidateLocalAuthorities validator)
        {
            It.IsNull(responseHelper)
                .AsGuard<ArgumentNullException>(nameof(responseHelper));
            It.IsNull(faultResponses)
                .AsGuard<ArgumentNullException>(nameof(faultResponses));
            It.IsNull(safeOperations)
                .AsGuard<ArgumentNullException>(nameof(safeOperations));
            It.IsNull(authorities)
                .AsGuard<ArgumentNullException>(nameof(authorities));
            It.IsNull(validator)
                .AsGuard<ArgumentNullException>(nameof(validator));

            Respond = responseHelper;
            Faults = faultResponses;
            SafeOperations = safeOperations;
            Authorities = authorities;
            Authority = validator;
        }

        /// <summary>
        /// get (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> GetAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope) =>
            await SafeOperations.Try(() => ProcessGetAuthorityFor(theTouchpoint, theLADCode, inScope), x => Faults.GetResponseFor(x, inScope));

        /// <summary>
        /// process, get (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> ProcessGetAuthorityFor(
            string theTouchpoint,
            string theLADCode,
            IScopeLoggingContext inScope)
        {
            await inScope.EnterMethod();

            It.IsEmpty(theTouchpoint)
                .AsGuard<ArgumentNullException>(nameof(theTouchpoint));
            It.IsEmpty(theLADCode)
                .AsGuard<ArgumentNullException>(nameof(theLADCode));

            await inScope.Information($"seeking the admin district: '{theLADCode}'");

            var result = await Authorities.Get(theLADCode);

            It.IsNull(result)
                .AsGuard<ArgumentNullException>(theLADCode);
            It.IsEmpty(result.LADCode)
                .AsGuard<ArgumentNullException>(theLADCode);

            await inScope.Information($"candidate search complete: '{result.LADCode}'");
            await inScope.Information($"validating touchpoint integrity: '{result.TouchpointID}' == '{theTouchpoint}'");

            (result.TouchpointID != theTouchpoint)
                .AsGuard<MalformedRequestException>(theTouchpoint);

            await inScope.Information($"preparing response...");

            var response = Respond.Ok().SetContent(result);

            await inScope.Information($"preparation complete...");

            await inScope.ExitMethod();

            return response;
        }

        /// <summary>
        /// add new authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="usingContent">using content</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> AddNewAuthorityFor(string theTouchpoint, string usingContent, IScopeLoggingContext inScope) =>
            await SafeOperations.Try(() => ProcessAddNewAuthorityFor(theTouchpoint, usingContent, inScope), x => Faults.GetResponseFor(x, inScope));

        /// <summary>
        /// process, add new authority for...
        /// submission choices...
        /// {"LADCode": "E00060060", "Name": "Widdicombe Sands" }
        /// {"TouchpointID":"0000000102", "LADCode": "E00060060", "Name": "Widdicombe Sands" }
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="usingContent">using content</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> ProcessAddNewAuthorityFor(
            string theTouchpoint,
            string usingContent,
            IScopeLoggingContext inScope)
        {
            await inScope.EnterMethod();

            It.IsEmpty(theTouchpoint)
                .AsGuard<ArgumentNullException>(nameof(theTouchpoint));
            It.IsEmpty(usingContent)
                .AsGuard<ArgumentNullException>(nameof(usingContent));

            await inScope.Information($"deserialising the submitted content: '{usingContent}'");

            var theCandidate = JsonConvert.DeserializeObject<IncomingLocalAuthority>(usingContent);

            It.IsNull(theCandidate)
                .AsGuard<MalformedRequestException>(nameof(ILocalAuthority.LADCode));

            await inScope.Information("deserialisation complete...");

            if (It.IsEmpty(theCandidate.TouchpointID))
            {
                await inScope.Information($"applying missing touchpoint details: '{theTouchpoint}'");
                theCandidate.TouchpointID = theTouchpoint;
            }

            await inScope.Information($"validating the admin district candidate: '{theCandidate.LADCode}'");

            await Authority.Validate(theCandidate);

            await inScope.Information($"validation complete...");
            await inScope.Information($"adding the admin district candidate: '{theCandidate.LADCode}'");

            var result = await Authorities.Add(theCandidate);

            await inScope.Information($"candidate addition complete...");
            await inScope.Information($"preparing response...");

            var response = Respond.Created().SetContent(result);

            await inScope.Information($"preparation complete...");
            await inScope.ExitMethod();

            return response;
        }

        /// <summary>
        /// delete (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> DeleteAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope) =>
            await SafeOperations.Try(() => ProcessDeleteAuthorityFor(theTouchpoint, theLADCode, inScope), x => Faults.GetResponseFor(x, inScope));

        /// <summary>
        /// process, delete (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> ProcessDeleteAuthorityFor(
            string theTouchpoint,
            string theLADCode,
            IScopeLoggingContext inScope)
        {
            await inScope.EnterMethod();

            It.IsEmpty(theTouchpoint)
                .AsGuard<ArgumentNullException>(nameof(theTouchpoint));
            It.IsEmpty(theLADCode)
                .AsGuard<ArgumentNullException>(nameof(theLADCode));

            await inScope.Information($"seeking the admin district: '{theLADCode}'");

            var result = await Authorities.Get(theLADCode);

            It.IsNull(result)
                .AsGuard<NoContentException>();
            It.IsEmpty(result.LADCode)
                .AsGuard<ArgumentNullException>(theLADCode);

            await inScope.Information($"candidate search complete: '{result.LADCode}'");
            await inScope.Information($"validating touchpoint integrity: '{result.TouchpointID}' == '{theTouchpoint}'");

            (result.TouchpointID != theTouchpoint)
                .AsGuard<NoContentException>(NoContentException.GetMessage(theTouchpoint));

            await inScope.Information($"deleting authority: '{result.Name}'");

            await Authorities.Delete(theLADCode);

            await inScope.Information($"preparing response...");

            var response = Respond.Ok();

            await inScope.Information($"preparation complete...");

            await inScope.ExitMethod();

            return response;
        }
    }
}
