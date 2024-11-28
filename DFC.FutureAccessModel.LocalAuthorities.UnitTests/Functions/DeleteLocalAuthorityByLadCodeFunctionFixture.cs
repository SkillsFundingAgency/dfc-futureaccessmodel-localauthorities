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
    public sealed class DeleteLocalAuthorityByLadCodeFunctionFixture :
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
            var logger = MakeStrictMock<ILogger<DeleteLocalAuthorityByLadCodeFunction>>();
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
            var adapter = MakeStrictMock<IManageLocalAuthorities>();
            var logger = MakeStrictMock<ILogger<DeleteLocalAuthorityByLadCodeFunction>>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new DeleteLocalAuthorityByLadCodeFunction(null, adapter, logger));
        }

        /// <summary>
        /// run with null adapter throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public void RunWithNullAdapterThrows()
        {
            // arrange
            var factory = MakeStrictMock<ICreateLoggingContextScopes>();
            var logger = MakeStrictMock<ILogger<DeleteLocalAuthorityByLadCodeFunction>>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new DeleteLocalAuthorityByLadCodeFunction(factory, null, logger));
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

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.Dispose());

            var logger = MakeStrictMock<ILogger<DeleteLocalAuthorityByLadCodeFunction>>();

            var sut = MakeSUT(logger);
            GetMock(sut.Factory)
                .Setup(x => x.BeginScopeFor(request, logger, "RunActionScope"))
                .Returns(Task.FromResult(scope));
            GetMock(sut.Adapter)
                .Setup(x => x.DeleteAuthorityFor(theTouchpoint, theAdminDistrict, scope))
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
        internal DeleteLocalAuthorityByLadCodeFunction MakeSUT(ILogger<DeleteLocalAuthorityByLadCodeFunction> logger)
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
        internal DeleteLocalAuthorityByLadCodeFunction MakeSUT(
            ICreateLoggingContextScopes factory,
            IManageLocalAuthorities adapter,
            ILogger<DeleteLocalAuthorityByLadCodeFunction> logger) =>
                new DeleteLocalAuthorityByLadCodeFunction(factory, adapter, logger);
    }
}
