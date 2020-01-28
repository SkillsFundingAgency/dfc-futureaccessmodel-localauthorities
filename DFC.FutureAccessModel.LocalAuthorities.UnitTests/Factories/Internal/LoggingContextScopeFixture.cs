using System;
using System.Net;
using System.Threading.Tasks;
using DFC.Common.Standard.Logging;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the logging context scope factory fixture
    /// </summary>
    public sealed class LoggingContextScopeFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IScopeLoggingContext>(MakeSUT());
        }

        /// <summary>
        /// correlation id is not empty
        /// </summary>
        [Fact]
        public void CorrelationIDIsNotEmpty() =>
            Assert.NotEqual(Guid.Empty, MakeSUT().CorrelationID);

        /// <summary>
        /// enter method, log helper meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task EnterMethodLogHelperMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            GetMock(sut.LoggerHelper)
                .Setup(x => x.LogInformationMessage(sut.Log, sut.CorrelationID, "entering method: 'EnterMethodLogHelperMeetsVerification'"));

            // act
            await sut.EnterMethod();

            // assert
            GetMock(sut.Log).VerifyAll();
            GetMock(sut.LoggerHelper).VerifyAll();
        }

        /// <summary>
        /// exit method, log helper meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ExitMethodLogHelperMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            GetMock(sut.LoggerHelper)
                .Setup(x => x.LogInformationMessage(sut.Log, sut.CorrelationID, "exiting method: 'ExitMethodLogHelperMeetsVerification'"));

            // act
            await sut.ExitMethod();

            // assert
            GetMock(sut.Log).VerifyAll();
            GetMock(sut.LoggerHelper).VerifyAll();
        }

        /// <summary>
        /// recording a message, log helper meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RecordingAMessageLogHelperMeetsVerification()
        {
            // arrange
            const string message = "recording a message";

            var sut = MakeSUT();
            GetMock(sut.LoggerHelper)
                .Setup(x => x.LogInformationMessage(sut.Log, sut.CorrelationID, message));

            // act
            await sut.Information(message);

            // assert
            GetMock(sut.Log).VerifyAll();
            GetMock(sut.LoggerHelper).VerifyAll();
        }

        /// <summary>
        /// recording an exception, log helper meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RecordingAnExceptionLogHelperMeetsVerification()
        {
            // arrange
            var exception = new MalformedRequestException();
            var sut = MakeSUT();
            GetMock(sut.LoggerHelper)
                .Setup(x => x.LogException(sut.Log, sut.CorrelationID, exception));

            // act
            await sut.ExceptionDetail(exception);

            // assert
            GetMock(sut.Log).VerifyAll();
            GetMock(sut.LoggerHelper).VerifyAll();
        }

        /// <summary>
        /// recording a message during dispose,log helper meets verification
        /// </summary>
        [Fact]
        public void RecordingAMessageDuringDisposeLogHelperMeetsVerification()
        {
            // arrange
            const string message = "request completed";

            var sut = MakeSUT();
            GetMock(sut.LoggerHelper)
                .Setup(x => x.LogInformationMessage(sut.Log, sut.CorrelationID, message));

            // act
            sut.Dispose();

            // assert
            GetMock(sut.Log).VerifyAll();
            GetMock(sut.LoggerHelper).VerifyAll();
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal LoggingContextScope MakeSUT()
        {
            var log = MakeStrictMock<ILogger>();
            var helper = MakeStrictMock<ILoggerHelper>();

            return MakeSUT(log, helper);
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>a document client factory</returns>
        internal LoggingContextScope MakeSUT(ILogger log, ILoggerHelper logHelper, string initialisingRoutine = null) =>
            new LoggingContextScope(log, logHelper, initialisingRoutine);
    }
}
