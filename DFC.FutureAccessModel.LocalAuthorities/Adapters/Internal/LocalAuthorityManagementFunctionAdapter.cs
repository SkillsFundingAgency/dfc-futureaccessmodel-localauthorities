﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using DFC.FutureAccessModel.LocalAuthorities.Storage;
using DFC.HTTP.Standard;
using Newtonsoft.Json;

namespace DFC.FutureAccessModel.LocalAuthorities.Adapters.Internal
{
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
            IStoreLocalAuthorities authorities)
        {
            It.IsNull(responseHelper)
                .AsGuard<ArgumentNullException>(nameof(responseHelper));
            It.IsNull(faultResponses)
                .AsGuard<ArgumentNullException>(nameof(faultResponses));
            It.IsNull(safeOperations)
                .AsGuard<ArgumentNullException>(nameof(safeOperations));
            It.IsNull(authorities)
                .AsGuard<ArgumentNullException>(nameof(authorities));

            Respond = responseHelper;
            Faults = faultResponses;
            SafeOperations = safeOperations;
            Authorities = authorities;
        }

        /// <summary>
        /// get (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> GetAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope) =>
            await SafeOperations.Try(
                () => ProcessGetAuthorityFor(theTouchpoint, theLADCode, inScope),
                x => Faults.GetResponseFor(x, inScope));

        /// <summary>
        /// process, get (the) local authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="theLADCode">the local adinistrative district code</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> ProcessGetAuthorityFor(string theTouchpoint, string theLADCode, IScopeLoggingContext inScope)
        {
            await inScope.EnterMethod();

            It.IsEmpty(theTouchpoint)
                .AsGuard<ArgumentNullException>(nameof(theTouchpoint));
            It.IsEmpty(theLADCode)
                .AsGuard<ArgumentNullException>(nameof(theLADCode));

            var result = await Authorities.Get(theLADCode);

            (result.TouchpointID != theTouchpoint)
                .AsGuard<MalformedRequestException>();

            await inScope.ExitMethod();

            return Respond.Ok().SetContent(result);
        }

        /// <summary>
        /// add new authority for...
        /// </summary>
        /// <param name="theTouchpoint">the touchpoint</param>
        /// <param name="usingContent">using content</param>
        /// <param name="inScope">in scope</param>
        /// <returns>the result of the operation</returns>
        public async Task<HttpResponseMessage> AddNewAuthorityFor(
            string theTouchpoint,
            string usingContent,
            IScopeLoggingContext inScope) =>
            await SafeOperations.Try(
                () => ProcessAddNewAuthorityFor(theTouchpoint, usingContent, inScope),
                x => Faults.GetResponseFor(x, inScope));

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

            var candidate = JsonConvert.DeserializeObject<LocalAuthority>(usingContent);

            if (It.IsEmpty(candidate.TouchpointID))
            {
                candidate.TouchpointID = theTouchpoint;
            }

            var result = await Authorities.Add(candidate);

            await inScope.ExitMethod();

            return Respond.Created().SetContent(result);
        }
    }
}