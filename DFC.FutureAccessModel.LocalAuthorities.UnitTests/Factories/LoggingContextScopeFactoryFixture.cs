using System;
using System.Net;
using System.Threading.Tasks;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
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
            var log = MakeStrictMock<ILogger>();

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
            var log = MakeStrictMock<ILogger>();

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
            var log = MakeStrictMock<ILogger>();

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
            var log = MakeStrictMock<ILogger>();

            // act
            _ = await sut.BeginScopeFor(request, log);

            // assert
            GetMock(sut.LoggerHelper)
                .Verify(x => x.LogInformationMessage(log, It.IsAny<Guid>(), It.IsAny<string>()), Times.Exactly(7));
        }

        /// <summary>
        /// make (a http candidate) request 
        /// </summary>
        /// <returns>a http request</returns>
        internal HttpRequest MakeRequest()
        {
            var connectionInfo = MakeStrictMock<ConnectionInfo>();
            GetMock(connectionInfo)
                .SetupGet(x => x.RemoteIpAddress)
                .Returns(IPAddress.Loopback);
            GetMock(connectionInfo)
                .SetupGet(x => x.RemotePort)
                .Returns(1234);

            var context = MakeStrictMock<HttpContext>();
            GetMock(context)
                .SetupGet(x => x.Connection)
                .Returns(connectionInfo);

            var headers = MakeStrictMock<IHeaderDictionary>();
            GetMock(headers)
                .Setup(x => x.ContainsKey(LoggingContextScopeFactory.HeaderAPIKey))
                .Returns(false);
            GetMock(headers)
                .Setup(x => x.ContainsKey(LoggingContextScopeFactory.HeaderVersion))
                .Returns(false);

            var request = MakeStrictMock<HttpRequest>();
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
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal LoggingContextScopeFactory MakeSUT()
        {
            var helper = MakeStrictMock<ILoggerHelper>();
            GetMock(helper)
                .Setup(x => x.LogInformationMessage(It.IsAny<ILogger>(), It.IsAny<Guid>(), It.IsAny<string>()));

            return MakeSUT(helper);
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>a document client factory</returns>
        internal LoggingContextScopeFactory MakeSUT(ILoggerHelper logHelper) =>
            new LoggingContextScopeFactory(logHelper);
    }
}
