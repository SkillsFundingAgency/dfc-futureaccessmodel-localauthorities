using System;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the storage path provider fixture
    /// </summary>
    public sealed class StoragePathProviderFixture :
        MoqTestingFixture
    {
        /// <summary>
        ///  the document store id key
        /// </summary>
        const string storeIDKey = "DocumentStoreID";

        /// <summary>
        /// the routing collection id key
        /// </summary>
        const string storeRoutingCollectionIDKey = "RoutingDetailCollectionID";

        /// <summary>
        /// the local authority collection id key
        /// </summary>
        const string storeLACollectionIDKey = "LocalAuthorityCollectionID";

        /// <summary>
        /// the (test) store name
        /// </summary>
        const string documentStoreName = "Store";

        /// <summary>
        /// the (test) area detail collection name 
        /// </summary>
        const string routingCollectionName = "Details";

        /// <summary>
        /// the (test) local authority collection name
        /// </summary>
        const string authorityCollectionName = "Authorities";

        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IProvideStoragePaths>(MakeSUT());
        }

        /// <summary>
        /// document store id key meets expectation
        /// </summary>
        [Fact]
        public void DocumentStoreIDKeyMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(storeIDKey, StoragePathProvider.DocumentStoreIDKey);
        }

        /// <summary>
        /// local authority collection id key meets expectation
        /// </summary>
        [Fact]
        public void LocalAuthorityCollectionIDKeyMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(storeLACollectionIDKey, StoragePathProvider.LocalAuthorityCollectionIDKey);
        }

        /// <summary>
        /// build with null settings throws
        /// </summary>
        [Fact]
        public void BuildWithNullSettingsThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(null));
        }

        /// <summary>
        /// build meets verification
        /// </summary>
        [Fact]
        public void BuildMeetsVerification()
        {
            // arrange / act
            var sut = MakeSUT();

            // assert
            GetMock(sut.Settings).VerifyAll();
        }

        /// <summary>
        /// build storage collection paths meet expectation
        /// </summary>
        [Fact]
        public void BuildStorageCollectionPathsMeetExpectation()
        {
            // arrange
            var sut = MakeSUT();

            // act / assert
            Assert.Equal("dbs/Store/colls/Authorities", sut.LocalAuthorityCollection.OriginalString);
        }

        /// <summary>
        /// get (the) local authority resource path for the admin district meets expectation
        /// </summary>
        [Fact]
        public void GetLocalAuthorityResourcePathForTheAdminDistrictMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();

            // act
            var result = sut.GetLocalAuthorityResourcePathFor("0000000001");

            // assert
            Assert.Equal("dbs/Store/colls/Authorities/docs/0000000001", result.OriginalString);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal StoragePathProvider MakeSUT()
        {
            var settings = MakeStrictMock<IProvideApplicationSettings>();
            GetMock(settings)
                .Setup(x => x.GetVariable(StoragePathProvider.DocumentStoreIDKey))
                .Returns(documentStoreName);
            GetMock(settings)
                .Setup(x => x.GetVariable(StoragePathProvider.LocalAuthorityCollectionIDKey))
                .Returns(authorityCollectionName);

            return MakeSUT(settings);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="paths">the storage paths provider</param>
        /// <param name="store">the document store</param>
        /// <returns>the system under test</returns>
        internal StoragePathProvider MakeSUT(
            IProvideApplicationSettings settings) =>
            new StoragePathProvider(settings);
    }
}
