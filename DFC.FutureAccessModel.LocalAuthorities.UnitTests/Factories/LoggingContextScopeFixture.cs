using DFC.Common.Standard.Logging;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
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
        /// enter method, logger meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task EnterMethodLoggerMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            SetupLogger(sut.Logger, LogLevel.Information, "entering method: 'EnterMethodLoggerMeetsVerification'");

            // act
            await sut.EnterMethod();

            // assert
            GetMock(sut.Logger).VerifyAll();
            VerifyLogger(sut.Logger, LogLevel.Information, "entering method: 'EnterMethodLoggerMeetsVerification'");
        }

        /// <summary>
        /// exit method, logger meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ExitMethodLoggerMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            SetupLogger(sut.Logger, LogLevel.Information, "exiting method: 'ExitMethodLoggerMeetsVerification'");

            // act
            await sut.ExitMethod();

            // assert
            GetMock(sut.Logger).VerifyAll();
            VerifyLogger(sut.Logger, LogLevel.Information, "exiting method: 'ExitMethodLoggerMeetsVerification'");
        }

        /// <summary>
        /// recording a message, logger meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RecordingAMessageLoggerMeetsVerification()
        {
            // arrange
            const string message = "recording a message";

            var sut = MakeSUT();
            SetupLogger(sut.Logger, LogLevel.Information, message);

            // act
            await sut.Information(message);

            // assert
            GetMock(sut.Logger).VerifyAll();
            VerifyLogger(sut.Logger, LogLevel.Information, message);
        }

        /// <summary>
        /// recording an exception, logger meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task RecordingAnExceptionLoggerMeetsVerification()
        {
            // arrange
            var exception = new MalformedRequestException();
            var sut = MakeSUT();

            SetupLogger(sut.Logger, LogLevel.Error, string.Empty, exception);

            // act
            await sut.ExceptionDetail(exception);

            // assert
            GetMock(sut.Logger).VerifyAll();
            VerifyLogger(sut.Logger, LogLevel.Error, string.Empty, exception);
        }

        /// <summary>
        /// recording a message during dispose,logger meets verification
        /// </summary>
        [Fact]
        public void RecordingAMessageDuringDisposeLoggerMeetsVerification()
        {
            // arrange
            const string message = "request completed";

            var sut = MakeSUT();
            SetupLogger(sut.Logger, LogLevel.Information, message);

            // act
            sut.Dispose();

            // assert
            GetMock(sut.Logger).VerifyAll();
            VerifyLogger(sut.Logger, LogLevel.Information, message);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal LoggingContextScope MakeSUT()
        {
            var log = MakeMock<ILogger>();            

            return MakeSUT(log);
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>a document client factory</returns>
        internal LoggingContextScope MakeSUT(ILogger log, string initialisingRoutine = null) =>
            new LoggingContextScope(log, initialisingRoutine);

        private void VerifyLogger(ILogger logger, LogLevel logLevel, string message, Exception exception = null)
        {
            GetMock(logger).Verify(l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => string.IsNullOrEmpty(message) ? true : v.ToString().Contains(message)),
                exception == null ? It.IsAny<Exception>() : exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(1));
        }

        private void SetupLogger(ILogger logger, LogLevel logLevel, string message, Exception exception = null)
        {
            GetMock(logger).Setup(l => l.Log(
                            logLevel,
                            It.IsAny<EventId>(),
                            It.Is<It.IsAnyType>((state, t) => string.IsNullOrEmpty(message) ? true : state.ToString().Contains(message)),
                            exception == null ? It.IsAny<Exception>() : exception,
                            It.IsAny<Func<It.IsAnyType, Exception, string>>()));
        }
    }
}
