using System;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
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
        /// get local authority for the admin district meets verification
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
                .Setup(x => x.GetDocument<LocalAuthority>(documentPath, "not_required"))
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
            var resourcePath = new Uri("any/old/resource/path", UriKind.Relative);
            var la = new IncomingLocalAuthority { TouchpointID = touchpoint };

            GetMock(sut.DocumentStore)
                .Setup(x => x.DocumentExists<LocalAuthority>(resourcePath, "not_required"))
                .Returns(Task.FromResult(true));

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
            var laResourcePath = new Uri("any/old/la/resource/path", UriKind.Relative);
            var la = new IncomingLocalAuthority { TouchpointID = touchpoint, LADCode = ladcode };

            GetMock(sut.StoragePaths)
                .Setup(x => x.GetLocalAuthorityResourcePathFor(ladcode))
                .Returns(laResourcePath);

            GetMock(sut.DocumentStore)
                .Setup(x => x.DocumentExists<LocalAuthority>(laResourcePath, "not_required"))
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
            const string pKey = "not_required";
            var sut = MakeSUT();
            var touchpoint = "any old touchpoint";
            var ladcode = "E000606060";
            var laResourcePath = new Uri("any/old/la/resource/path", UriKind.Relative);
            var la = new IncomingLocalAuthority { TouchpointID = touchpoint, LADCode = ladcode };

            GetMock(sut.StoragePaths)
                .Setup(x => x.GetLocalAuthorityResourcePathFor(ladcode))
                .Returns(laResourcePath);
            GetMock(sut.StoragePaths)
                .SetupGet(x => x.LocalAuthorityCollection)
                .Returns(laResourcePath);

            GetMock(sut.DocumentStore)
                .Setup(x => x.DocumentExists<LocalAuthority>(laResourcePath, pKey))
                .Returns(Task.FromResult(false));
            GetMock(sut.DocumentStore)
                .Setup(x => x.AddDocument(la, It.IsAny<Uri>()))
                .Returns(Task.FromResult(la));

            // act
            var result = await sut.Add(la);

            // assert
            GetMock(sut.StoragePaths).VerifyAll();
            GetMock(sut.DocumentStore).VerifyAll();
            Assert.Equal(la, result);
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
            var documentPath = new Uri("/", UriKind.Relative);

            GetMock(sut.DocumentStore)
                .Setup(x => x.DeleteDocument(documentPath, "not_required"))
                .Returns(Task.CompletedTask);
            GetMock(sut.StoragePaths)
                .Setup(x => x.GetLocalAuthorityResourcePathFor(theAdminDistrict))
                .Returns(documentPath);

            // act
            await sut.Delete(theAdminDistrict);

            // assert
            GetMock(sut.DocumentStore).VerifyAll();
            GetMock(sut.StoragePaths).VerifyAll();
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
