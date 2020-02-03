using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    /// <summary>
    /// the get area routing detail by touchpoint ID function fixture
    /// </summary>
    public sealed class PostLocalAuthorityFunctionFixture :
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
            await Assert.ThrowsAsync<ArgumentNullException>(() => PostLocalAuthorityFunction.Run(null, trace, "", factory, adapter));
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
            await Assert.ThrowsAsync<ArgumentNullException>(() => PostLocalAuthorityFunction.Run(request, null, "", factory, adapter));
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
            await Assert.ThrowsAsync<ArgumentNullException>(() => PostLocalAuthorityFunction.Run(request, trace, "", null, adapter));
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
            await Assert.ThrowsAsync<ArgumentNullException>(() => PostLocalAuthorityFunction.Run(request, trace, "", factory, null));
        }

        /// <summary>
        /// run meets expectation
        /// </summary>
        [Fact]
        public async Task RunMeetsExpectation()
        {
            // arrange
            // const string theAdminDistrict = "E1234567";
            const string theTouchpoint = "00000000112";
            const string localAuthority = "{ \"LADCode\": \"E1234567\", \"Name\": \"Buckingham and Berks\" }";

            var request = MakeStrictMock<HttpRequest>();
            GetMock(request)
                .Setup(x => x.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(localAuthority)));

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
                .Setup(x => x.AddNewAuthorityFor(theTouchpoint, localAuthority, scope))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)));

            // act
            var result = await PostLocalAuthorityFunction.Run(request, trace, theTouchpoint, factory, adapter);

            // assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.IsAssignableFrom<HttpResponseMessage>(result);
        }
    }
}
