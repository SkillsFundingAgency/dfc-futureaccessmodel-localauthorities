using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the logging context scope factory fixture
    /// </summary>
    public sealed class LoggingContextScopeFactoryFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<ICreateLoggingContextScopes>(MakeSUT());
        }

        /// <summary>
        /// header API key meets expectation
        /// </summary>
        [Fact]
        public void HeaderAPIKeyMeetsExpectation() =>
            Assert.Equal("Ocp-Apim-Subscription-Key", LoggingContextScopeFactory.HeaderAPIKey);

        /// <summary>
        /// header version meets expectation
        /// </summary>
        [Fact]
        public void HeaderVersionMeetsExpectation() =>
            Assert.Equal("Version", LoggingContextScopeFactory.HeaderVersion);

        /// <summary>
        /// value not found meets expectation
        /// </summary>
        [Fact]
        public void ValueNotFoundMeetsExpectation() =>
            Assert.Equal("(not found)", LoggingContextScopeFactory.ValueNotFound);

        /// <summary>
        /// begin (logging context) scope for the request returns something
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task BeginScopeForTheRequestReturnsSomething()
        {
            // arrange
            var sut = MakeSUT();
            var request = MakeRequest();
            var log = MakeMock<ILogger>();

            SetupLogger(log, LogLevel.Information, string.Empty);

            // act
            var scope = await sut.BeginScopeFor(request, log);

            // assert
            Assert.NotNull(scope);
        }

        /// <summary>
        /// begin (logging context) scope for the request returns a logging scope
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task BeginScopeForTheRequestReturnsALoggingScope()
        {
            // arrange
            var sut = MakeSUT();
            var request = MakeRequest();
            var log = MakeMock<ILogger>();

            SetupLogger(log, LogLevel.Information, string.Empty);

            // act
            var scope = await sut.BeginScopeFor(request, log);

            // assert
            Assert.IsType<LoggingContextScope>(scope);
        }

        /// <summary>
        /// begin (logging context) scope for the request returns a
        /// instance mathching the logging context scope contract
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task BeginScopeForTheRequestReturnsAnAssignableContract()
        {
            // arrange
            var sut = MakeSUT();
            var request = MakeRequest();
            var log = MakeMock<ILogger>();

            SetupLogger(log, LogLevel.Information, string.Empty);

            // act
            var scope = await sut.BeginScopeFor(request, log);

            // assert
            Assert.IsAssignableFrom<IScopeLoggingContext>(scope);
        }

        /// <summary>
        /// begin scope for the request records 7 items
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BeginScopeForTheRequestRecordsSevenItems()
        {
            // arrange
            var sut = MakeSUT();
            var request = MakeRequest();
            var log = MakeMock<ILogger>();
            SetupLogger(log, LogLevel.Information, string.Empty);

            // act
            _ = await sut.BeginScopeFor(request, log);

            // assert            
            var noOfTimes = 7;
            VerifyLogger(log, LogLevel.Information, string.Empty, noOfTimes);
        }

        /// <summary>
        /// make (a http candidate) request 
        /// </summary>
        /// <returns>a http request</returns>
        internal HttpRequest MakeRequest()
        {
            var connectionInfo = MakeMock<ConnectionInfo>();
            GetMock(connectionInfo)
                .SetupGet(x => x.RemoteIpAddress)
                .Returns(IPAddress.Loopback);
            GetMock(connectionInfo)
                .SetupGet(x => x.RemotePort)
                .Returns(1234);

            var context = MakeMock<HttpContext>();
            GetMock(context)
                .SetupGet(x => x.Connection)
                .Returns(connectionInfo);

            var headers = MakeMock<IHeaderDictionary>();
            GetMock(headers)
                .Setup(x => x.ContainsKey(LoggingContextScopeFactory.HeaderAPIKey))
                .Returns(false);
            GetMock(headers)
                .Setup(x => x.ContainsKey(LoggingContextScopeFactory.HeaderVersion))
                .Returns(false);

            var request = MakeMock<HttpRequest>();
            GetMock(request)
                .SetupGet(x => x.QueryString)
                .Returns(new QueryString("?blah=IAmAQueryString"));
            GetMock(request)
                .SetupGet(x => x.ContentType)
                .Returns("moq/test");
            GetMock(request)
                .SetupGet(x => x.Method)
                .Returns("GET");
            GetMock(request)
                .SetupGet(x => x.Headers)
                .Returns(headers);
            GetMock(request)
                .SetupGet(x => x.HttpContext)
                .Returns(context);

            return request;
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>a document client factory</returns>
        internal LoggingContextScopeFactory MakeSUT() =>
            new LoggingContextScopeFactory();

        private void VerifyLogger(ILogger logger, LogLevel logLevel, string message, int callCount = 1, Exception exception = null)
        {
            GetMock(logger).Verify(l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => string.IsNullOrEmpty(message) ? true : v.ToString().Contains(message)),
                exception == null ? It.IsAny<Exception>() : exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(callCount));
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
