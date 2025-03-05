using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
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
            var logger = MakeMock<ILogger<GetLocalAuthorityByLadCodeFunction>>();
            var sut = MakeSUT(logger);

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Run(null, "", ""));
        }

        /// <summary>
        /// run with null factory throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public void RunWithNullFactoryThrows()
        {
            // arrange
            var adapter = MakeMock<IManageLocalAuthorities>();
            var logger = MakeMock<ILogger<GetLocalAuthorityByLadCodeFunction>>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new GetLocalAuthorityByLadCodeFunction(null, adapter, logger));
        }

        /// <summary>
        /// run with null adapter throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public void RunWithNullAdapterThrows()
        {
            // arrange
            var factory = MakeMock<ICreateLoggingContextScopes>();
            var logger = MakeMock<ILogger<GetLocalAuthorityByLadCodeFunction>>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new GetLocalAuthorityByLadCodeFunction(factory, null, logger));
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

            var request = MakeMock<HttpRequest>();
            var logger = MakeMock<ILogger<GetLocalAuthorityByLadCodeFunction>>();

            var scope = MakeMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.Dispose());

            var sut = MakeSUT(logger);
            GetMock(sut.Factory)
                .Setup(x => x.BeginScopeFor(request, logger, "RunActionScope"))
                .Returns(Task.FromResult(scope));
            GetMock(sut.Adapter)
                .Setup(x => x.GetAuthorityFor(theTouchpoint, theAdminDistrict, scope))
                .Returns(Task.FromResult<IActionResult>(new OkResult()));

            // act
            var result = await sut.Run(request, theTouchpoint, theAdminDistrict);

            // assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal GetLocalAuthorityByLadCodeFunction MakeSUT(ILogger<GetLocalAuthorityByLadCodeFunction> logger)
        {
            var factory = MakeMock<ICreateLoggingContextScopes>();
            var adapter = MakeMock<IManageLocalAuthorities>();

            return MakeSUT(factory, adapter, logger);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="factory">(the logging scope) factory</param>
        /// <param name="adapter">(the local authority management) adapter</param>
        /// <returns>the system under test</returns>
        internal GetLocalAuthorityByLadCodeFunction MakeSUT(
            ICreateLoggingContextScopes factory,
            IManageLocalAuthorities adapter,
            ILogger<GetLocalAuthorityByLadCodeFunction> logger) =>
                new GetLocalAuthorityByLadCodeFunction(factory, adapter, logger);
    }
}
