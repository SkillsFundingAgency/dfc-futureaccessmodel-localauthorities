using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Functions
{
    /// <summary>
    /// the api (document generating) definition test fixture
    /// </summary>
    public sealed class ApiDefinitionFunctionFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the api definition title meets expectation
        /// </summary>
        [Fact]
        public void TitleMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal("Admin District support for the Omni Channel Area Routing API", ApiDefinitionFunction.ApiTitle);
        }

        /// <summary>
        /// the api definition version meets expectation
        /// </summary>
        [Fact]
        public void VersionMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal("1.0.0", ApiDefinitionFunction.ApiVersion);
        }

        /// <summary>
        /// the api definition name meets expectation
        /// </summary>
        [Fact]
        public void NameMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal("api-definition", ApiDefinitionFunction.ApiDefinitionName);
        }

        /// <summary>
        /// the api definition description is not empty
        /// </summary>
        [Fact]
        public void DescriptionIsNotEmpty()
        {
            // arrange / act / assert
            Assert.NotSame(string.Empty, ApiDefinitionFunction.ApiDescription);
        }

        /// <summary>
        /// the api definition run routine throws with a null http request
        /// </summary>
        [Fact]
        public async Task RunWithNullRequestThrows()
        {
            // arrange
            var sut = MakeSUT();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Run(null));
        }

        /// <summary>
        /// the api definition run routine throws with a null document generator
        /// </summary>
        [Fact]
        public void RunWithNullDocumentGeneratorThrows()
        {
            // arrange
            var request = MakeStrictMock<HttpRequest>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ApiDefinitionFunction(null));
        }

        /// <summary>
        /// run meets expectation
        /// </summary>
        [Fact]
        public async Task RunMeetsExpectation()
        {
            const string documentContent = "document returned from generator";

            // arrange
            var request = MakeStrictMock<HttpRequest>();

            var sut = MakeSUT();
            // the mock expects the defaults to be sent in on optional values
            GetMock(sut.Generator)
                .Setup(x => x.GenerateSwaggerDocument(
                    request,
                    ApiDefinitionFunction.ApiTitle,
                    ApiDefinitionFunction.ApiDescription,
                    ApiDefinitionFunction.ApiDefinitionName,
                    ApiDefinitionFunction.ApiVersion,
                    Moq.It.IsAny<Assembly>(),
                    false, // include subcontractor id, optional parameter
                    false, // include touchpoint id, optional parameter
                    "/api/")) // api route prefix => /api, optional parameter
                .Returns(documentContent);

            // act
            var result = await sut.Run(request);
            var resultResponse = result as OkObjectResult;

            // assert
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, resultResponse.StatusCode);
            Assert.Equal(documentContent, resultResponse.Value);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal ApiDefinitionFunction MakeSUT() =>
            new ApiDefinitionFunction(MakeStrictMock<ISwaggerDocumentGenerator>());
    }
}
