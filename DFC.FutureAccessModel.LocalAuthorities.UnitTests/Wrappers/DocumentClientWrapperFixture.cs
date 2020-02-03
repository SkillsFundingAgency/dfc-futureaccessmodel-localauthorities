using System;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Wrappers.Internal
{
    public class DocumentClientWrapperFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IWrapDocumentClient>(MakeSUT());
        }

        /// <summary>
        /// build with null client throws
        /// </summary>
        [Fact]
        public void BuildWithNullClientThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new DocumentClientWrapper(null));
        }

        /// <summary>
        /// build with client meets expectation
        /// </summary>
        [Fact]
        public void BuildWithClientMeetsExpectation()
        {
            // arrange
            var client = MakeStrictMock<IDocumentClient>();

            // act
            var sut = new DocumentClientWrapper(client);

            // assert
            Assert.Equal(client, sut.Client);
        }

        /// <summary>
        /// create document (async) meets expectation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateDocumentAsyncMeetsExpectation()
        {
            // arrange
            const string keyValue = "0000123";
            var sut = MakeSUT();
            var document = new LocalAuthority { LADCode = keyValue };
            var collectionUri = new Uri("dbs/areas/colls/routing", UriKind.Relative);
            var documentUri = new Uri($"dbs/areas/colls/routing/docs/{keyValue}", UriKind.Relative);

            GetMock(sut.Client)
                .Setup(x => x.CreateDocumentAsync(collectionUri, document, null, false, default))
                .Returns(Task.FromResult(new ResourceResponse<Document>(new Document())));
            GetMock(sut.Client)
                .Setup(x => x.ReadDocumentAsync<LocalAuthority>(documentUri, null, default))
                .Returns(Task.FromResult(new DocumentResponse<LocalAuthority>(document)));

            // act
            var result = await sut.CreateDocumentAsync(collectionUri, document);

            // assert
            Assert.IsAssignableFrom<ILocalAuthority>(result);
        }

        /// <summary>
        /// document exists (async) false with null response meets expectation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DocumentExistsAsyncFalseWithNullResponseMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var documentUri = new Uri("dbs/areas/colls/routing/docs/0000123", UriKind.Relative);

            GetMock(sut.Client)
                .Setup(x => x.ReadDocumentAsync(documentUri, null, default))
                .Returns(Task.FromResult<ResourceResponse<Document>>(null));

            // act
            var result = await sut.DocumentExistsAsync(documentUri);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// document exists (async) true meets expectation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DocumentExistsAsyncTrueMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var documentUri = new Uri("dbs/areas/colls/routing/docs/0000123", UriKind.Relative);

            GetMock(sut.Client)
                .Setup(x => x.ReadDocumentAsync(documentUri, null, default))
                .Returns(Task.FromResult(new ResourceResponse<Document>(new Document())));

            // act
            var result = await sut.DocumentExistsAsync(documentUri);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// read document (async) with valid URI meets expectation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ReadDocumentAsyncWithValidURIMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var documentUri = new Uri("dbs/areas/colls/routing/docs/0000123", UriKind.Relative);
            var document = new LocalAuthority();

            GetMock(sut.Client)
                .Setup(x => x.ReadDocumentAsync<LocalAuthority>(documentUri, null, default))
                .Returns(Task.FromResult(new DocumentResponse<LocalAuthority>(document)));

            // act
            var result = await sut.ReadDocumentAsync<LocalAuthority>(documentUri);

            // assert
            Assert.IsAssignableFrom<ILocalAuthority>(result);
        }

        /// <summary>
        /// make document path for key value and collection meets expectation
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="collectionPath"></param>
        [Theory]
        [InlineData("0000123", "dbs/areas/colls/routing")]
        [InlineData("E0600032", "dbs/regions/colls/authorities")]
        public void MakeDocumentPathForKeyValueAndCollectionMeetsExpectation(string keyValue, string collectionPath)
        {
            // arrange
            var document = new LocalAuthority { LADCode = keyValue };
            var collectionUri = new Uri(collectionPath, UriKind.Relative);
            var documentPath = $"{collectionPath}/docs/{keyValue}";

            var sut = MakeSUT();

            // act
            var result = sut.MakeDocumentPathFor(document, collectionUri);

            // assert
            Assert.Equal(documentPath, result.OriginalString);
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>a system under test</returns>
        internal DocumentClientWrapper MakeSUT() =>
            new DocumentClientWrapper(MakeStrictMock<IDocumentClient>());
    }
}
