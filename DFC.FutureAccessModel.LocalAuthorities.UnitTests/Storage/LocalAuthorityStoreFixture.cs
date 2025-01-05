using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Wrappers;
using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Storage.Internal
{
    /// <summary>
    /// the local authority store fixture
    /// </summary>
    public sealed class LocalAuthorityStoreFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IStoreLocalAuthorities>(MakeSUT());
        }

        /// <summary>
        /// build with null cosmos db wrapper throws
        /// </summary>
        [Fact]
        public void BuildWithNullCosmosDbWrapperThrows()
        {
            // act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(null));
        }

        /// <summary>
        /// build meets verification
        /// </summary>
        [Fact]
        public void BuildMeetsVerification()
        {
            // arrange            
            var cosmosDbWrapper = MakeMock<IWrapCosmosDbClient>();

            // act
            var sut = MakeSUT(cosmosDbWrapper);

            // assert
            GetMock(sut.CosmosDbWrapper).VerifyAll();
        }

        /// <summary>
        /// get local authority for the admin district meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task GetLocalAuthorityForTheAdminDistrictMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            const string theAdminDistrict = "any old district";

            GetMock(sut.CosmosDbWrapper)
                .Setup(x => x.GetLocalAuthorityAsync(theAdminDistrict, "not_required"))
                .Returns(Task.FromResult(new LocalAuthority()));

            // act
            var result = await sut.Get(theAdminDistrict);

            // assert
            GetMock(sut.CosmosDbWrapper).VerifyAll();
            Assert.IsAssignableFrom<ILocalAuthority>(result);
        }

        /// <summary>
        /// add local authority with null candidate throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task AddLocalAuthorityWithNullCandidateThrows()
        {
            // arrange
            var sut = MakeSUT();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(null));
        }

        /// <summary>
        /// add local authority with null touchpoint throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task AddLocalAuthorityWithNullTouchpointThrowsThrows()
        {
            // arrange
            var sut = MakeSUT();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(new IncomingLocalAuthority()));
        }

        /// <summary>
        /// add local authority with no LAD Code throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task AddLocalAuthorityWithNoLADCodeThrows()
        {
            // arrange
            var sut = MakeSUT();
            var touchpoint = "any old touchpoint";
            var la = new IncomingLocalAuthority { TouchpointID = touchpoint };

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Add(la));
        }

        /// <summary>
        /// add local authority with existing LAD Code throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task AddLocalAuthorityWithExistingLADCodeThrows()
        {
            // arrange
            var sut = MakeSUT();
            var touchpoint = "any old touchpoint";
            var ladcode = "E000606060";
            var la = new IncomingLocalAuthority { TouchpointID = touchpoint, LADCode = ladcode };

            GetMock(sut.CosmosDbWrapper)
                .Setup(x => x.LocalAuthorityExistsAsync(la.LADCode, "not_required"))
                .Returns(Task.FromResult(true));

            // act / assert
            await Assert.ThrowsAsync<ConflictingResourceException>(() => sut.Add(la));
        }

        /// <summary>
        /// add local authority with existing LAD Code throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task AddLocalAuthorityWithValidAuthorityMeetsVerification()
        {
            // arrange
            const string partitionKey = "not_required";
            var sut = MakeSUT();
            var touchpoint = "any old touchpoint";
            var ladcode = "E000606060";
            var la = new IncomingLocalAuthority { TouchpointID = touchpoint, LADCode = ladcode };
            var mockItemResponse = new Mock<ItemResponse<LocalAuthority>>();
            mockItemResponse.SetupGet(x => x.Resource).Returns(la);

            GetMock(sut.CosmosDbWrapper)
                .Setup(x => x.LocalAuthorityExistsAsync(la.LADCode, partitionKey))
                .Returns(Task.FromResult(false));

            GetMock(sut.CosmosDbWrapper)
                .Setup(x => x.CreateLocalAuthorityAsync(la, partitionKey))
                .Returns(Task.FromResult(mockItemResponse.Object));

            // act
            var result = await sut.Add(la);

            // assert
            GetMock(sut.CosmosDbWrapper).VerifyAll();
            Assert.Equal(la, result);
        }

        /// <summary>
        /// delete local authority with null admin district (LAD Code) throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task DeleteLocalAuthorityWithNullAdminDistrictThrows()
        {
            // arrange
            var sut = MakeSUT();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Delete(null));
        }

        /// <summary>
        /// delete local authority with admin district (LAD Code) not already existing throws
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task DeleteLocalAuthorityWithAdminDistrictNotExistingThrows()
        {
            // arrange
            var sut = MakeSUT();
            const string theAdminDistrict = "any old district";

            GetMock(sut.CosmosDbWrapper)
                .Setup(x => x.LocalAuthorityExistsAsync(theAdminDistrict, "not_required"))
                .Returns(Task.FromResult(false));

            // act / assert
            await Assert.ThrowsAsync<NoContentException>(() => sut.Delete(theAdminDistrict));
        }

        /// <summary>
        /// delete local authority for the admin district meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task DeleteLocalAuthorityForTheAdminDistrictMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            const string theAdminDistrict = "any old district";

            GetMock(sut.CosmosDbWrapper)
                .Setup(x => x.LocalAuthorityExistsAsync(theAdminDistrict, "not_required"))
                .Returns(Task.FromResult(true));

            // act
            await sut.Delete(theAdminDistrict);

            // assert
            GetMock(sut.CosmosDbWrapper).VerifyAll();
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal LocalAuthorityStore MakeSUT()
        {
            var cosmosDbWrapper = MakeMock<IWrapCosmosDbClient>();

            return MakeSUT(cosmosDbWrapper);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="paths">the storage paths provider</param>
        /// <param name="store">the document store</param>
        /// <returns>the system under test</returns>
        internal LocalAuthorityStore MakeSUT(
            IWrapCosmosDbClient cosmosDbWrapper) =>
            new LocalAuthorityStore(cosmosDbWrapper);
    }
}
