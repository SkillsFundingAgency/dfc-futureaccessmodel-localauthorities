using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    /// <summary>
    /// the get local authority by lad code function fixture
    /// </summary>
    public sealed class GetLocalAuthorityByLadCodeFunctionFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// run with null request throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RunWithNullRequestThrows()
        {
            // arrange
            var trace = MakeStrictMock<ILogger>();
            var factory = MakeStrictMock<ICreateLoggingContextScopes>();
            var adapter = MakeStrictMock<IManageLocalAuthorities>();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => GetLocalAuthorityByLadCodeFunction.Run(null, trace, "", "", factory, adapter));
        }

        /// <summary>
        /// run with null trace throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RunWithNullTraceThrows()
        {
            // arrange
            var request = MakeStrictMock<HttpRequest>();
            var factory = MakeStrictMock<ICreateLoggingContextScopes>();
            var adapter = MakeStrictMock<IManageLocalAuthorities>();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => GetLocalAuthorityByLadCodeFunction.Run(request, null, "", "", factory, adapter));
        }

        /// <summary>
        /// run with null factory throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RunWithNullFactoryThrows()
        {
            // arrange
            var request = MakeStrictMock<HttpRequest>();
            var trace = MakeStrictMock<ILogger>();
            var adapter = MakeStrictMock<IManageLocalAuthorities>();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => GetLocalAuthorityByLadCodeFunction.Run(request, trace, "", "", null, adapter));
        }

        /// <summary>
        /// run with null adapter throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RunWithNullAdapterThrows()
        {
            // arrange
            var request = MakeStrictMock<HttpRequest>();
            var trace = MakeStrictMock<ILogger>();
            var factory = MakeStrictMock<ICreateLoggingContextScopes>();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => GetLocalAuthorityByLadCodeFunction.Run(request, trace, "", "", factory, null));
        }

        /// <summary>
        /// run meets expectation
        /// </summary>
        [Fact]
        public async Task RunMeetsExpectation()
        {
            // arrange
            const string theAdminDistrict = "E1234567";
            const string theTouchpoint = "00000000112";

            var request = MakeStrictMock<HttpRequest>();
            var trace = MakeStrictMock<ILogger>();

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.Dispose());

            var factory = MakeStrictMock<ICreateLoggingContextScopes>();
            GetMock(factory)
                .Setup(x => x.BeginScopeFor(request, trace, "Run"))
                .Returns(Task.FromResult(scope));

            var adapter = MakeStrictMock<IManageLocalAuthorities>();
            GetMock(adapter)
                .Setup(x => x.GetAuthorityFor(theTouchpoint, theAdminDistrict, scope))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            // act
            var result = await GetLocalAuthorityByLadCodeFunction.Run(request, trace, theTouchpoint, theAdminDistrict, factory, adapter);

            // assert
            Assert.IsAssignableFrom<HttpResponseMessage>(result);
        }
    }
}
