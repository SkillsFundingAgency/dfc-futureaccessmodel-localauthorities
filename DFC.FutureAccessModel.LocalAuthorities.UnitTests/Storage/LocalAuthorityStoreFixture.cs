using System;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using Moq;
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
        /// build with null paths throws
        /// </summary>
        [Fact]
        public void BuildWithNullPathsThrows()
        {
            // arrange
            var store = MakeStrictMock<IStoreDocuments>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(null, store));
        }

        /// <summary>
        /// build with null store throws
        /// </summary>
        [Fact]
        public void BuildWithNullStoreThrows()
        {
            // arrange
            var paths = MakeStrictMock<IProvideStoragePaths>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(paths, null));
        }

        /// <summary>
        /// build meets verification
        /// </summary>
        [Fact]
        public void BuildMeetsVerification()
        {
            // arrange
            var paths = MakeStrictMock<IProvideStoragePaths>();
            var store = MakeStrictMock<IStoreDocuments>();

            // act
            var sut = MakeSUT(paths, store);

            // assert
            GetMock(sut.DocumentStore).VerifyAll();
            GetMock(sut.StoragePaths).VerifyAll();
        }

        /// <summary>
        /// get local authrotiy for the admin district meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task GetLocalAuthorityForTheAdminDistrictMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            const string theAdminDistrict = "any old district";
            var documentPath = new Uri("/", UriKind.Relative);

            GetMock(sut.DocumentStore)
                .Setup(x => x.GetDocument<LocalAuthority>(documentPath))
                .Returns(Task.FromResult(new LocalAuthority()));
            GetMock(sut.StoragePaths)
                .Setup(x => x.GetLocalAuthorityResourcePathFor(theAdminDistrict))
                .Returns(documentPath);

            // act
            var result = await sut.Get(theAdminDistrict);

            // assert
            GetMock(sut.DocumentStore).VerifyAll();
            GetMock(sut.StoragePaths).VerifyAll();
            Assert.IsAssignableFrom<ILocalAuthority>(result);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal LocalAuthorityStore MakeSUT()
        {
            var paths = MakeStrictMock<IProvideStoragePaths>();
            var store = MakeStrictMock<IStoreDocuments>();

            return MakeSUT(paths, store);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="paths">the storage paths provider</param>
        /// <param name="store">the document store</param>
        /// <returns>the system under test</returns>
        internal LocalAuthorityStore MakeSUT(
            IProvideStoragePaths paths,
            IStoreDocuments store) =>
            new LocalAuthorityStore(paths, store);
    }
}
