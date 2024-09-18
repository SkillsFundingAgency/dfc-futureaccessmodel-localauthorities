using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
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
            var logger = MakeStrictMock<ILogger<PostLocalAuthorityFunction>>();
            var sut = MakeSUT(logger);            

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Run(null, ""));
        }        

        /// <summary>
        /// run with null factory throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public void RunWithNullFactoryThrows()
        {
            // arrange
            var logger = MakeStrictMock<ILogger<PostLocalAuthorityFunction>>();
            var adapter = MakeStrictMock<IManageLocalAuthorities>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new PostLocalAuthorityFunction(null, adapter, logger));
        }

        /// <summary>
        /// run with null adapter throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public void RunWithNullAdapterThrows()
        {
            // arrange
            var logger = MakeStrictMock<ILogger<PostLocalAuthorityFunction>>();
            var factory = MakeStrictMock<ICreateLoggingContextScopes>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new PostLocalAuthorityFunction(factory, null, logger));
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

            //var request = MakeStrictMock<HttpRequest>();
            var request = new Mock<HttpRequest>().Object;
            GetMock(request)
                .Setup(x => x.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(localAuthority)));            

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.Dispose());

            var logger = MakeStrictMock<ILogger<PostLocalAuthorityFunction>>();
            var sut = MakeSUT(logger);
            GetMock(sut.Factory)
                .Setup(x => x.BeginScopeFor(request, logger, "RunActionScope"))
                .Returns(Task.FromResult(scope));

            GetMock(sut.Adapter)
                .Setup(x => x.AddNewAuthorityFor(theTouchpoint, localAuthority, scope))
                .Returns(Task.FromResult<IActionResult>(new JsonResult(localAuthority, new JsonSerializerSettings())
                { StatusCode = (int)HttpStatusCode.Created }));

            // act
            var result = await sut.Run(request, theTouchpoint);
            var resultResponse = result as JsonResult;

            // assert
            Assert.Equal((int)HttpStatusCode.Created, resultResponse.StatusCode);
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="logger">logger</param>
        /// <returns>the system under test</returns>
        internal PostLocalAuthorityFunction MakeSUT(ILogger<PostLocalAuthorityFunction> logger)
        {
            var factory = MakeStrictMock<ICreateLoggingContextScopes>();
            var adapter = MakeStrictMock<IManageLocalAuthorities>();

            return MakeSUT(factory, adapter, logger);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authority management) adapter</param>
        /// <returns>the system under test</returns>
        internal PostLocalAuthorityFunction MakeSUT(
            ICreateLoggingContextScopes factory,
            IManageLocalAuthorities adapter,
            ILogger<PostLocalAuthorityFunction> logger) =>
                new PostLocalAuthorityFunction(factory, adapter, logger);
    }
}
