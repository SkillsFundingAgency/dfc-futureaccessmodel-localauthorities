using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using DFC.FutureAccessModel.LocalAuthorities.Wrappers;
using Microsoft.Azure.Documents;
using Moq;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Storage.Internal
{
    /// <summary>
    /// the document store fixture
    /// </summary>
    public sealed class DocumentStoreFixture :
        MoqTestingFixture
    {
        /// <summary>
        ///  the endpoint address key
        /// </summary>
        const string storeEndpointAddressKey = "DocumentStoreEndpointAddress";

        /// <summary>
        /// the document store key
        /// </summary>
        const string storeAccountKey = "DocumentStoreAccountKey";

        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IStoreDocuments>(MakeSUT());
        }

        /// <summary>
        /// the document store endpoint address key meets expectation
        /// </summary>
        [Fact]
        public void EndpointAddressKeyMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(storeEndpointAddressKey, DocumentStore.DocumentStoreEndpointAddressKey);
        }

        /// <summary>
        /// the document store account key meets expectation
        /// </summary>
        [Fact]
        public void AccountKeyMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(storeAccountKey, DocumentStore.DocumentStoreAccountKey);
        }

        /// <summary>
        /// build with null settings throws
        /// </summary>
        [Fact]
        public void BuildWithNullSettingsThrows()
        {
            // arrange
            var factory = MakeStrictMock<ICreateDocumentClients>();
            var safeOps = MakeStrictMock<IProvideSafeOperations>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(null, factory, safeOps));
        }

        /// <summary>
        /// build with null factory throws
        /// </summary>
        [Fact]
        public void BuildWithNullFactoryThrows()
        {
            // arrange
            var settings = MakeStrictMock<IProvideApplicationSettings>();
            var safeOps = MakeStrictMock<IProvideSafeOperations>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(settings, null, safeOps));
        }

        /// <summary>
        /// build with null safe operations throws
        /// </summary>
        [Fact]
        public void BuildWithNullSafeOperationsThrows()
        {
            // arrange
            var settings = MakeStrictMock<IProvideApplicationSettings>();
            var factory = MakeStrictMock<ICreateDocumentClients>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => MakeSUT(settings, factory, null));
        }

        /// <summary>
        /// build failure meets expectation
        /// </summary>
        /// <param name="endpointAddress">the test endpoint address</param>
        /// <param name="accountKey">the test account key</param>
        /// <param name="expectedException">the expected exception</param>
        [Theory]
        [InlineData(null, null, typeof(ArgumentNullException))]
        [InlineData(null, "anyOldAccountKey", typeof(ArgumentNullException))]
        [InlineData("anyOldEndpoint", null, typeof(ArgumentNullException))]
        [InlineData("anyOldEndpoint", "anyOldAccountKey", typeof(UriFormatException))]
        public void BuildFailureMeetsExpectation(string endpointAddress, string accountKey, Type expectedException)
        {
            // arrange
            var settings = MakeStrictMock<IProvideApplicationSettings>();
            GetMock(settings)
                .Setup(x => x.GetVariable(storeEndpointAddressKey))
                .Returns(endpointAddress);
            GetMock(settings)
                .Setup(x => x.GetVariable(storeAccountKey))
                .Returns(accountKey);

            var factory = MakeStrictMock<ICreateDocumentClients>();
            var safeOps = MakeStrictMock<IProvideSafeOperations>();

            // act / assert
            Assert.Throws(expectedException, () => MakeSUT(settings, factory, safeOps));
        }

        /// <summary>
        /// build meets verification
        /// </summary>
        [Fact]
        public void BuildMeetsVerification()
        {
            // arrange
            const string accountKeyValue = "anyOldAccountKey";

            var settings = MakeStrictMock<IProvideApplicationSettings>();
            GetMock(settings)
                .Setup(x => x.GetVariable(storeEndpointAddressKey))
                .Returns("http://localhost:123/");
            GetMock(settings)
                .Setup(x => x.GetVariable(storeAccountKey))
                .Returns(accountKeyValue);

            var factory = MakeStrictMock<ICreateDocumentClients>();
            GetMock(factory)
                .Setup(x => x.CreateClient(It.IsAny<Uri>(), accountKeyValue))
                .Returns<IDocumentClient>(null);

            var safeOps = MakeStrictMock<IProvideSafeOperations>();

            // act
            var sut = MakeSUT(settings, factory, safeOps);

            // assert
            GetMock(settings).VerifyAll();
            GetMock(factory).VerifyAll();
            GetMock(safeOps).VerifyAll();
        }

        /// <summary>
        /// document exists meets expectation
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DocumentExistsMeetsExpectation(bool expectation)
        {
            // arrange
            var sut = MakeSUT();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.SafeOperations)
                .Setup(x => x.Try(It.IsAny<Func<Task<bool>>>(), It.IsAny<Func<Exception, Task<bool>>>()))
                .Returns(Task.FromResult(expectation));

            // act
            var result = await sut.DocumentExists<LocalAuthority>(testPath, "not_required");

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// process document exists meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ProcessDocumentExistsMeetsVerification(bool expectation)
        {
            // arrange
            const string pKey = "any old partition key";
            var sut = MakeSUT();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.Client)
                .Setup(x => x.DocumentExistsAsync<LocalAuthority>(testPath, pKey))
                .Returns(Task.FromResult(expectation));

            // act
            var result = await sut.ProcessDocumentExists<LocalAuthority>(testPath, pKey);

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// add document meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task AddDocumentMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            var modelItem = new LocalAuthority();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.SafeOperations)
                .Setup(x => x.Try(It.IsAny<Func<Task<LocalAuthority>>>(), It.IsAny<Func<Exception, Task<LocalAuthority>>>()))
                .Returns(Task.FromResult(modelItem));

            // act
            await sut.AddDocument(modelItem, testPath);

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// process add document meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ProcessAddDocumentMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            var modelItem = new LocalAuthority();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.Client)
                .Setup(x => x.CreateDocumentAsync(testPath, modelItem))
                .Returns(Task.FromResult(modelItem));

            // act
            await sut.ProcessAddDocument(testPath, modelItem);

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// get document meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task GetDocumentMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.SafeOperations)
                .Setup(x => x.Try(It.IsAny<Func<Task<LocalAuthority>>>(), It.IsAny<Func<Exception, Task<LocalAuthority>>>()))
                .Returns(Task.FromResult(new LocalAuthority()));

            // act
            var result = await sut.GetDocument<LocalAuthority>(testPath, "not_required");

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
            Assert.IsType<LocalAuthority>(result);
        }

        /// <summary>
        /// process get document meets expectation
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ProcessGetDocumentMeetsVerification()
        {
            // arrange
            const string pKey = "any old partition key";
            var sut = MakeSUT();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.Client)
                .Setup(x => x.ReadDocumentAsync<LocalAuthority>(testPath, pKey))
                .Returns(Task.FromResult(new LocalAuthority()));

            // act
            var result = await sut.ProcessGetDocument<LocalAuthority>(testPath, pKey);

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
            Assert.IsType<LocalAuthority>(result);
        }

        /// <summary>
        /// delete document meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task DeleteDocumentMeetsVerification()
        {
            // arrange
            const string pKey = "any old partition key";
            var sut = MakeSUT();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.SafeOperations)
                .Setup(x => x.Try(It.IsAny<Func<Task>>(), It.IsAny<Func<Exception, Task>>()))
                .Returns(Task.CompletedTask);

            // act
            await sut.DeleteDocument(testPath, pKey);

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// process delete document meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ProcessDeleteDocumentMeetsVerification()
        {
            // arrange
            const string pKey = "any old partition key";
            var sut = MakeSUT();
            var testPath = new Uri("http://blahStore/blahCollection/blahID");

            GetMock(sut.Client)
                .Setup(x => x.DeleteDocumentAsync(testPath, pKey))
                .Returns(Task.CompletedTask);

            // act
            await sut.ProcessDeleteDocument(testPath, pKey);

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// process document error handler throws malformed request exception for null incoming exception
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ProcessDocumentErrorHandlerThrowsMalformedRequestForIncomingNull()
        {
            // arrange
            var sut = MakeSUT();

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessDocumentErrorHandler<LocalAuthority>(null));
        }

        /// <summary>
        /// process document error handler meets expectation
        /// </summary>
        /// <param name="httpCode">the http code</param>
        /// <param name="expectedException">the expected exception</param>
        /// <returns>the currently running (test) task</returns>
        [Theory]
        [InlineData(HttpStatusCode.NotFound, typeof(NoContentException))]
        [InlineData(HttpStatusCode.TooManyRequests, typeof(MalformedRequestException))]
        public async Task ProcessDocumentErrorHandlerMeetsExpectation(HttpStatusCode httpCode, Type expectedException)
        {
            // arrange
            var sut = MakeSUT();
            var exception = MakeDocumentClientException(httpCode);

            // act / assert
            await Assert.ThrowsAsync(expectedException, () => sut.ProcessDocumentErrorHandler<LocalAuthority>(exception));
        }

        /// <summary>
        /// process document error handler with null http status code
        /// this shouldn't happen and is the test in place only for code coverage
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ProcessDocumentErrorHandlerWithNullHttpStatusCode()
        {
            // arrange
            var sut = MakeSUT();
            var exception = MakeDocumentClientException(null);

            // act / assert
            await Assert.ThrowsAsync<DocumentClientException>(() => sut.ProcessDocumentErrorHandler<LocalAuthority>(exception));
        }

        /// <summary>
        /// process document client error with null passes witout event
        /// this shouldn't happen and is the test in place only for code coverage
        /// </summary>
        [Fact]
        public void ProcessDocumentClientErrorWithNullPassesWithoutEvent()
        {
            // arrange
            var sut = MakeSUT();

            // act
            sut.ProcessDocumentClientError(null);

            // assert
            GetMock(sut.Client).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// make (a) document client exception
        /// all constructors have been internalised for some reason so we can't mock or 'new' up
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <returns>a document client exception</returns>
        internal DocumentClientException MakeDocumentClientException(HttpStatusCode? httpStatusCode)
        {
            var type = typeof(DocumentClientException);
            var name = type.FullName;
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var parms = new object[] { new Error(), null, httpStatusCode };

            var instance = type.Assembly.CreateInstance(name, false, flags, null, parms, null, null);

            return (DocumentClientException)instance;
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal DocumentStore MakeSUT()
        {
            const string accountKeyValue = "anyOldAccountKey";

            var settings = MakeStrictMock<IProvideApplicationSettings>();
            GetMock(settings)
                .Setup(x => x.GetVariable(storeEndpointAddressKey))
                .Returns("http://localhost:123/");
            GetMock(settings)
                .Setup(x => x.GetVariable(storeAccountKey))
                .Returns(accountKeyValue);

            var factory = MakeStrictMock<ICreateDocumentClients>();
            GetMock(factory)
                .Setup(x => x.CreateClient(It.IsAny<Uri>(), accountKeyValue))
                .Returns(MakeStrictMock<IWrapDocumentClient>());

            var safeOperator = MakeStrictMock<IProvideSafeOperations>();

            return MakeSUT(settings, factory, safeOperator);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="settings">the application settings provider</param>
        /// <param name="factory">the document client factory</param>
        /// <param name="safeOperator">the safe operations provider</param>
        /// <returns>the system under test</returns>
        internal DocumentStore MakeSUT(
            IProvideApplicationSettings settings,
            ICreateDocumentClients factory,
            IProvideSafeOperations safeOperator) =>
                new DocumentStore(settings, factory, safeOperator);
    }
}
