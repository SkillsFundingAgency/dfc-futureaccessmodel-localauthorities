using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the fault response provider fixture
    /// </summary>
    public sealed class FaultResponseProviderFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IProvideFaultResponses>(MakeSUT());
        }

        /// <summary>
        /// get response for the exception meets expectation
        /// </summary>
        /// <param name="testException">the test exception (type)</param>
        /// <param name="expectedState">the expected http response state</param>
        /// <param name="expectedMessage">the expected message recording</param>
        /// <returns>the current (test) task</returns>
        [Theory]
        [InlineData(typeof(MalformedRequestException), HttpStatusCode.BadRequest, "")]
        [InlineData(typeof(NoContentException), HttpStatusCode.NoContent, "Resource does not exist")]
        [InlineData(typeof(UnprocessableEntityException), HttpStatusCode.UnprocessableEntity, "")]
        public async Task GetResponseForTheExceptionMeetsExpectation(Type testException, HttpStatusCode expectedState, string expectedMessage)
        {
            // arrange
            var sut = MakeSUT();
            var logger = MakeLoggingContext($"Exception of type '{testException.FullName}' was thrown.");
            GetMock(logger)
                .Setup(x => x.Information(expectedMessage))
                .Returns(Task.CompletedTask);

            var exception = (Exception)testException.Assembly.CreateInstance(testException.FullName);

            // act
            var result = await sut.GetResponseFor(exception, logger);

            // assert
            var statusCode = (int)expectedState;
            switch (testException)
            {
                case var _ when testException == typeof(MalformedRequestException):
                    Assert.IsType<BadRequestObjectResult>(result);
                    Assert.Equal(statusCode, (result as BadRequestObjectResult).StatusCode);
                    break;

                case var _ when testException == typeof(NoContentException):
                    Assert.IsType<NoContentResult>(result);
                    Assert.Equal(statusCode, (result as NoContentResult).StatusCode);
                    break;

                case var _ when testException == typeof(UnprocessableEntityException):
                    Assert.IsType<UnprocessableEntityObjectResult>(result);
                    Assert.Equal(statusCode, (result as UnprocessableEntityObjectResult).StatusCode);
                    break;
            }
        }

        /// <summary>
        /// get resonse for unknown exception meets expectation
        /// </summary>
        /// <param name="testException">the test exception (type)</param>
        /// <returns>the current (test) task</returns>
        [Theory]
        [InlineData(typeof(ArgumentNullException), HttpStatusCode.InternalServerError, "Value cannot be null.")]
        [InlineData(typeof(ArgumentException), HttpStatusCode.InternalServerError, "Value does not fall within the expected range.")]
        [InlineData(typeof(InvalidOperationException), HttpStatusCode.InternalServerError, "Operation is not valid due to the current state of the object.")]
        public async Task GetResponseForUnknownExceptionMeetsExpectation(Type testException, HttpStatusCode expectedState, string expectedMessage)
        {
            // arrange
            var sut = MakeSUT();

            var exception = (Exception)testException.Assembly.CreateInstance(testException.FullName);

            var logger = MakeLoggingContext($"Exception of type '{testException.FullName}' was thrown.");
            GetMock(logger)
                .Setup(x => x.ExceptionDetail(exception))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.GetResponseFor(exception, logger);
            var resultResponse = result as InternalServerErrorResult;

            // assert
            Assert.Equal((int)expectedState, resultResponse.StatusCode);            
            Assert.Equal(expectedMessage, exception.Message);
        }

        /// <summary>
        /// malformed meets expectation
        /// </summary>
        [Fact]
        public void MalformedMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var exception = new Exception();

            // act
            var result = FaultResponseProvider.Malformed(exception);
            var resultResponse = result as BadRequestObjectResult;

            // assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultResponse.StatusCode);
        }

        /// <summary>
        /// conflicted meets expectation
        /// </summary>
        [Fact]
        public void ConflictedMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var exception = new Exception();

            // act
            var result = FaultResponseProvider.Conflicted(exception);
            var resultResponse = result as ConflictObjectResult;

            // assert
            Assert.Equal((int)HttpStatusCode.Conflict, resultResponse.StatusCode);
        }

        /// <summary>
        /// no content meets expectation
        /// </summary>
        [Fact]
        public void NoContentMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var exception = new Exception();

            // act
            var result = FaultResponseProvider.NoContent(exception);
            var resultResponse = result as NoContentResult;

            // assert
            Assert.Equal((int)HttpStatusCode.NoContent, resultResponse.StatusCode);
        }

        /// <summary>
        /// unprocessable entity meets expectation
        /// </summary>
        [Fact]
        public void UnprocessableEntityMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var exception = new Exception();

            // act
            var result = FaultResponseProvider.UnprocessableEntity(exception);
            var resultResponse = result as UnprocessableEntityObjectResult;

            // assert
            Assert.Equal((int)HttpStatusCode.UnprocessableContent, resultResponse.StatusCode);
        }

        /// <summary>
        /// unknown meets expectation
        /// </summary>
        [Fact]
        public void UnknownErrorMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var exception = new Exception();

            // act
            var result = FaultResponseProvider.UnknownError(exception);
            var resultResponse = result as InternalServerErrorResult;

            // assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultResponse.StatusCode);
        }

        /// <summary>
        /// make scope logging context 
        /// </summary>
        /// <param name="itemBeingRecorded"></param>
        /// <returns>a logging context scope</returns>
        internal IScopeLoggingContext MakeLoggingContext(string itemBeingRecorded)
        {
            var logger = MakeMock<IScopeLoggingContext>();
            GetMock(logger)
                .Setup(x => x.Information(itemBeingRecorded))
                .Returns(Task.CompletedTask);

            return logger;
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal FaultResponseProvider MakeSUT() =>
            new FaultResponseProvider();
    }
}
