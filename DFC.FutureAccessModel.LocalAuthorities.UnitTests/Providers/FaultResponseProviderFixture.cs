using System;
using System.Net;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
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
        [InlineData(typeof(UnauthorizedException), HttpStatusCode.Unauthorized, "")]
        [InlineData(typeof(AccessForbiddenException), HttpStatusCode.Forbidden, "Insufficient access to this resource")]
        [InlineData(typeof(UnprocessableEntityException), HttpStatusCode.UnprocessableEntity, "{ \"errors\": [{  }] }")]
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
            Assert.Equal(expectedState, result.StatusCode);
            Assert.Equal(expectedMessage, await result.Content.ReadAsStringAsync());
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

            // assert
            Assert.Equal(expectedState, result.StatusCode);
            Assert.Equal(expectedMessage, await result.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// make scope logging context 
        /// </summary>
        /// <param name="itemBeingRecorded"></param>
        /// <returns>a logging context scope</returns>
        internal IScopeLoggingContext MakeLoggingContext(string itemBeingRecorded)
        {
            var logger = MakeStrictMock<IScopeLoggingContext>();
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
