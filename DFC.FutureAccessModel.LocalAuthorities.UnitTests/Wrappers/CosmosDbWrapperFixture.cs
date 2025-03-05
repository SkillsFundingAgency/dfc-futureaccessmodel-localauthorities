using DFC.FutureAccessModel.LocalAuthorities.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using Microsoft.Identity.Client;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Wrappers.Internal
{
    public class CosmosDbWrapperFixture :
        MoqTestingFixture
    {
        private readonly Mock<CosmosClient> _mockCosmosClient;
        private readonly Mock<IOptions<ConfigurationSettings>> _mockConfigSettings;
        private readonly Mock<ILogger<CosmosDbWrapper>> _mockLogger;

        const string DocumentStoreID = "database";
        const string LocalAuthorityCollectionID = "localauthority";

        public CosmosDbWrapperFixture()
        {
            _mockCosmosClient = new Mock<CosmosClient>();
            _mockConfigSettings = new Mock<IOptions<ConfigurationSettings>>();
            _mockLogger = new Mock<ILogger<CosmosDbWrapper>>();

            var configSettings = new ConfigurationSettings
            {
                DocumentStoreID = DocumentStoreID,
                DocumentStoreAccountKey = "sdafsdkfjsdalfkjasdfkld",
                DocumentStoreEndpointAddress = "asdflksjadfksdjfklsd",
                LocalAuthorityCollectionID = LocalAuthorityCollectionID
            };

            var mockContainer = new Mock<Container>();
            var mockItemResponse = new Mock<ItemResponse<LocalAuthority>>();

            _mockCosmosClient.Setup(x => x.GetContainer(configSettings.DocumentStoreID, configSettings.LocalAuthorityCollectionID)).Returns(mockContainer.Object);
            mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<LocalAuthority>(), new PartitionKey("not_required"), null, It.IsAny<CancellationToken>())).ReturnsAsync(mockItemResponse.Object);

            _mockConfigSettings.SetupGet(c => c.Value).Returns(configSettings);
        }

        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IWrapCosmosDbClient>(MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object));
        }

        /// <summary>
        /// build with null client throws
        /// </summary>
        [Fact]
        public void BuildWithNullClientThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(null, _mockConfigSettings.Object, _mockLogger.Object));
        }

        /// <summary>
        /// build with null config settings throws
        /// </summary>
        [Fact]
        public void BuildWithNullConfigSettingsThrows()
        {
            var cosmosClient = new Mock<CosmosClient>();
            var logger = new Mock<ILogger<CosmosDbWrapper>>();

            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(cosmosClient.Object, null, logger.Object));
        }

        /// <summary>
        /// build with null logger throws
        /// </summary>
        [Fact]
        public void BuildWithNullLoggerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, null));
        }

        /// <summary>
        /// get local authority (async) meets expectation
        /// </summary>
        /// <returns>local authority</returns>
        [Fact]
        public async Task GetLocalAuthorityAsyncMeetsExpectation()
        {
            // arrange
            const string keyValue = "0000000209";
            const string partitionKey = "not_required";

            var localAuthority = new LocalAuthority
            {
                TouchpointID = keyValue,
                LADCode = "E123456",
                Name = "Buckingham and Berks testing"
            };

            var mockItemResponse = new Mock<ItemResponse<LocalAuthority>>();
            mockItemResponse.SetupGet(x => x.Resource).Returns(localAuthority);

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(x => x.ReadItemAsync<LocalAuthority>(localAuthority.LADCode, new PartitionKey(partitionKey), null, default))
                .ReturnsAsync(mockItemResponse.Object);

            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);

            // act
            var result = await sut.GetLocalAuthorityAsync(localAuthority.LADCode, partitionKey);

            // assert
            Assert.IsAssignableFrom<LocalAuthority>(result);
            Assert.NotNull(result);
            VerifyLogger(_mockLogger, LogLevel.Information, string.Empty, 2);
        }

        /// <summary>
        /// get local authority (async) returns null value when local authority not found
        /// </summary>
        /// <returns>local authority</returns>
        [Fact]
        public async Task GetLocalAuthorityAsyncReturnsNullWhenLocalAuthorityNotFound()
        {
            // arrange
            const string keyValue = "0000000209";
            const string partitionKey = "not_required";

            var localAuthority = new LocalAuthority
            {
                TouchpointID = keyValue,
                LADCode = "E123456",
                Name = "Buckingham and Berks testing"
            };

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(x => x.ReadItemAsync<LocalAuthority>(localAuthority.LADCode, new PartitionKey(partitionKey), null, default))
                .ThrowsAsync(new CosmosException("Testing exception", HttpStatusCode.NotFound, 0, "0", 0.0));

            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);

            // act
            var result = await sut.GetLocalAuthorityAsync(localAuthority.LADCode, partitionKey);

            // assert
            Assert.Null(result);
            VerifyLogger(_mockLogger, LogLevel.Warning, string.Empty, 1);
        }

        /// <summary>
        /// get local authority (async) returns null value when ReadItemAsync method throws exception
        /// </summary>
        /// <returns>local authority</returns>
        [Fact]
        public async Task GetLocalAuthorityAsyncReturnsNullWhenReadItemAsyncMethodThrowsException()
        {
            // arrange
            const string keyValue = "0000000209";
            const string partitionKey = "not_required";

            var localAuthority = new LocalAuthority
            {
                TouchpointID = keyValue,
                LADCode = "E123456",
                Name = "Buckingham and Berks testing"
            };

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(x => x.ReadItemAsync<LocalAuthority>(localAuthority.LADCode, new PartitionKey(partitionKey), null, default))
                .ThrowsAsync(new Exception("Testing exception"));

            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);

            // act
            var result = await sut.GetLocalAuthorityAsync(localAuthority.LADCode, partitionKey);

            // assert
            Assert.Null(result);
            VerifyLogger(_mockLogger, LogLevel.Error, string.Empty, 1);
        }

        /// <summary>
        /// create local authority (async) meets expectation
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task CreateLocalAuthorityAsyncMeetsExpectation()
        {
            // arrange
            const string keyValue = "0000000209";
            const string partitionKey = "not_required";

            var localAuthority = new LocalAuthority
            {
                TouchpointID = keyValue,
                LADCode = "E123456",
                Name = "Buckingham and Berks testing"
            };

            var mockItemResponse = new Mock<ItemResponse<LocalAuthority>>();
            mockItemResponse.SetupGet(x => x.Resource).Returns(localAuthority);

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(x => x.CreateItemAsync(localAuthority, new PartitionKey(partitionKey), null, default))
                .ReturnsAsync(mockItemResponse.Object);

            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);

            // act
            var result = await sut.CreateLocalAuthorityAsync(localAuthority, partitionKey);

            // assert
            Assert.IsAssignableFrom<ItemResponse<LocalAuthority>>(result);
            VerifyLogger(_mockLogger, LogLevel.Information, string.Empty, 2);
        }

        /// <summary>
        /// create local authority (async) returns null value when CreateItemAsync method throws exception
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task CreateLocalAuthorityAsyncReturnsNullWhenCreateMethodThrowsException()
        {
            // arrange
            const string keyValue = "0000000209";
            const string partitionKey = "not_required";

            var localAuthority = new LocalAuthority
            {
                TouchpointID = keyValue,
                LADCode = "E123456",
                Name = "Buckingham and Berks testing"
            };

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(x => x.CreateItemAsync(It.IsAny<LocalAuthority>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Testing exception"));

            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(mockContainer.Object);

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);

            // act
            var result = await sut.CreateLocalAuthorityAsync(localAuthority, partitionKey);

            // assert
            Assert.Null(result);
            VerifyLogger(_mockLogger, LogLevel.Error, string.Empty, 1);
        }

        /// <summary>
        /// CreateLocalAuthorityAsync method returns exception when input parameter LocalAuthority is null
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task CreateLocalAuthorityAsyncThrowsExceptionWhenLocalAuthorityObjectIsNull()
        {
            // arrange            
            const string partitionKey = "not_required";

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);


            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.CreateLocalAuthorityAsync(null, partitionKey));
        }

        /// <summary>
        /// delete local authority (async) meets expectation
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task DeleteLocalAuthorityAsyncMeetsExpectation()
        {
            // arrange
            const string keyValue = "0000000209";
            const string partitionKey = "not_required";

            var localAuthority = new LocalAuthority
            {
                TouchpointID = keyValue,
                LADCode = "E123456",
                Name = "Buckingham and Berks testing"
            };

            var mockItemResponse = new Mock<ItemResponse<LocalAuthority>>();
            mockItemResponse.SetupGet(x => x.Resource).Returns(localAuthority);

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(x => x.DeleteItemAsync<LocalAuthority>(localAuthority.LADCode, new PartitionKey(partitionKey), null, default))
                .ReturnsAsync(mockItemResponse.Object);

            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockContainer.Object);

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);

            // act
            var result = await sut.DeleteLocalAuthorityAsync(localAuthority.LADCode, partitionKey);

            // assert
            Assert.IsAssignableFrom<ItemResponse<LocalAuthority>>(result);
            VerifyLogger(_mockLogger, LogLevel.Information, string.Empty, 2);
        }

        /// <summary>
        /// delete local authority (async) returns null value when CreateItemAsync method throws exception
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task DeleteLocalAuthorityAsyncReturnsNullWhenDeleteItemAsyncMethodThrowsException()
        {
            // arrange
            const string keyValue = "0000000209";
            const string partitionKey = "not_required";

            var localAuthority = new LocalAuthority
            {
                TouchpointID = keyValue,
                LADCode = "E123456",
                Name = "Buckingham and Berks testing"
            };

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(x => x.DeleteItemAsync<LocalAuthority>(localAuthority.LADCode, It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Delete exception"));

            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(mockContainer.Object);

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);

            // act
            var result = await sut.DeleteLocalAuthorityAsync(localAuthority.LADCode, partitionKey);

            // assert
            Assert.Null(result);
            VerifyLogger(_mockLogger, LogLevel.Error, string.Empty, 1);
        }

        /// <summary>
        /// DeleteLocalAuthorityAsync method returns exception when input parameter LocalAuthority is null
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task DeleteLocalAuthorityAsyncThrowsExceptionWhenAdminDistrictCodeIsNull()
        {
            // arrange            
            const string partitionKey = "not_required";

            var sut = MakeSUT(_mockCosmosClient.Object, _mockConfigSettings.Object, _mockLogger.Object);


            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.DeleteLocalAuthorityAsync(null, partitionKey));
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>a system under test</returns>
        internal CosmosDbWrapper MakeSUT(CosmosClient client, IOptions<ConfigurationSettings> configOptions, ILogger<CosmosDbWrapper> logger) =>
            new CosmosDbWrapper(client, configOptions, logger);

        private void VerifyLogger(Mock<ILogger<CosmosDbWrapper>> logger, LogLevel logLevel, string message, int callCount = 1, Exception exception = null)
        {
            logger.Verify(l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => string.IsNullOrEmpty(message) ? true : v.ToString().Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(callCount));
        }
    }
}
