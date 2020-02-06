using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using DFC.FutureAccessModel.LocalAuthorities.Storage;
using DFC.HTTP.Standard;
using Moq;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Adapters.Internal
{
    /// <summary>
    /// local authority management function adapter fixture
    /// </summary>
    public sealed class LocalAuthorityManagementFunctionAdapterFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// build with null response helper throws
        /// </summary>
        [Fact]
        public void BuildWithNullResponseHelperThrows()
        {
            // arrange
            var store = MakeStrictMock<IStoreLocalAuthorities>();
            var faults = MakeStrictMock<IProvideFaultResponses>();
            var safe = MakeStrictMock<IProvideSafeOperations>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LocalAuthorityManagementFunctionAdapter(null, faults, safe, store));
        }

        /// <summary>
        /// build with null fault response provider throws
        /// </summary>
        [Fact]
        public void BuildWithNullFaultsResponseProviderThrows()
        {
            // arrange
            var store = MakeStrictMock<IStoreLocalAuthorities>();
            var helper = MakeStrictMock<IHttpResponseMessageHelper>();
            var safe = MakeStrictMock<IProvideSafeOperations>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LocalAuthorityManagementFunctionAdapter(helper, null, safe, store));
        }

        /// <summary>
        /// build with null safe opeations throws
        /// </summary>
        [Fact]
        public void BuildWithNullSafeOperationsThrows()
        {
            // arrange
            var store = MakeStrictMock<IStoreLocalAuthorities>();
            var helper = MakeStrictMock<IHttpResponseMessageHelper>();
            var faults = MakeStrictMock<IProvideFaultResponses>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LocalAuthorityManagementFunctionAdapter(helper, faults, null, store));
        }

        /// <summary>
        /// build with null (document) storage (provider) throws
        /// </summary>
        [Fact]
        public void BuildWithNullStorageProviderThrows()
        {
            // arrange
            var helper = MakeStrictMock<IHttpResponseMessageHelper>();
            var faults = MakeStrictMock<IProvideFaultResponses>();
            var safe = MakeStrictMock<IProvideSafeOperations>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LocalAuthorityManagementFunctionAdapter(helper, faults, safe, null));
        }

        /// <summary>
        /// get local authority for, meets verification
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAuthorityForMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            var scope = MakeStrictMock<IScopeLoggingContext>();

            GetMock(sut.SafeOperations)
                .Setup(x => x.Try(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<Func<Exception, Task<HttpResponseMessage>>>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            // act
            var result = await sut.GetAuthorityFor(string.Empty, string.Empty, scope);

            // assert
            Assert.IsAssignableFrom<HttpResponseMessage>(result);
            GetMock(sut.Authorities).VerifyAll();
            GetMock(sut.Respond).VerifyAll();
            GetMock(sut.Faults).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// process get authority for invalid items id meets expectation
        /// </summary>
        /// <param name="touchpointID"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("", "any old content")]
        [InlineData(null, "any old content")]
        [InlineData("any old touchpoint", "")]
        [InlineData("any old touchpoint", null)]
        public async Task ProcessGetAuthorityForInvalidItemMeetsExpectation(string touchpointID, string theContent)
        {
            // arrange
            var sut = MakeSUT();

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.EnterMethod("ProcessGetAuthorityFor"))
                .Returns(Task.CompletedTask);

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessGetAuthorityFor(touchpointID, theContent, scope));
        }

        /// <summary>
        /// process get authority for invalid items throws malformed request
        /// </summary>
        /// <param name="location">the location</param>
        /// <returns></returns>
        [Fact]
        public async Task ProcessGetAuthorityForInvalidItemsThrowsMalformedRequest()
        {
            // arrange
            const string theAdminDistrict = "E1234567";
            const string theTouchpoint = "00000000112";
            var localAuthority = new LocalAuthority
            {
                TouchpointID = "any old touchpoint",
                LADCode = null
            };

            var sut = MakeSUT();

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.EnterMethod("ProcessGetAuthorityFor"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"seeking the admin district: '{theAdminDistrict}'"))
                .Returns(Task.CompletedTask);
            GetMock(sut.Authorities)
                .Setup(x => x.Get(theAdminDistrict))
                .Returns(Task.FromResult<ILocalAuthority>(localAuthority));

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessGetAuthorityFor(theTouchpoint, theAdminDistrict, scope));
        }

        /// <summary>
        /// process get authority for valid items meets verification
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProcessGetAuthorityForValidItemsMeetsVerification()
        {
            // arrange
            const string theAdminDistrict = "E1234567";
            const string theTouchpoint = "00000000112";
            var localAuthority = new LocalAuthority
            {
                TouchpointID = theTouchpoint,
                LADCode = theAdminDistrict
            };

            var sut = MakeSUT();

            GetMock(sut.Authorities)
                .Setup(x => x.Get(theAdminDistrict))
                .Returns(Task.FromResult<ILocalAuthority>(localAuthority));

            GetMock(sut.Respond)
                .Setup(x => x.Ok())
                .Returns(new HttpResponseMessage());

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.EnterMethod("ProcessGetAuthorityFor"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"seeking the admin district: '{theAdminDistrict}'"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"candidate search complete: '{theAdminDistrict}'"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"validating touchpoint integrity: '{localAuthority.TouchpointID}' == '{theTouchpoint}'"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"preparing response..."))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"preparation complete..."))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.ExitMethod("ProcessGetAuthorityFor"))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.ProcessGetAuthorityFor(theTouchpoint, theAdminDistrict, scope);

            // assert
            Assert.IsAssignableFrom<HttpResponseMessage>(result);
            GetMock(sut.Authorities).VerifyAll();
            GetMock(sut.Respond).VerifyAll();
            GetMock(sut.Faults).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// add new authority for, meets verification
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddNewAuthorityForMeetsVerification()
        {
            // arrange
            var sut = MakeSUT();
            var scope = MakeStrictMock<IScopeLoggingContext>();

            GetMock(sut.SafeOperations)
                .Setup(x => x.Try(It.IsAny<Func<Task<HttpResponseMessage>>>(), It.IsAny<Func<Exception, Task<HttpResponseMessage>>>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            // act
            var result = await sut.AddNewAuthorityFor(string.Empty, string.Empty, scope);

            // assert
            Assert.IsAssignableFrom<HttpResponseMessage>(result);
            GetMock(sut.Authorities).VerifyAll();
            GetMock(sut.Respond).VerifyAll();
            GetMock(sut.Faults).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// process add routing detail using missing content throws
        /// </summary>
        /// <param name="theContent">the content</param>
        /// <returns>the currently running (test) task</returns>
        [Theory]
        [InlineData("", "{}")]
        [InlineData(null, "{}")]
        [InlineData("{}", "")]
        [InlineData("{}", null)]
        public async Task ProcessAddNewAuthorityForWithMissingItemsThrows(string theTouchpoint, string theContent)
        {
            // arrange
            var sut = MakeSUT();

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.EnterMethod("ProcessAddNewAuthorityFor"))
                .Returns(Task.CompletedTask);

            // act / assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessAddNewAuthorityFor(theTouchpoint, theContent, scope));
        }

        /// <summary>
        /// process add routing detail using invalid content throws
        /// </summary>
        /// <param name="theContent">the content</param>
        /// <returns>the currently running (test) task</returns>
        [Theory]
        [InlineData("0000000129","{ }")]
        [InlineData("0000000119", "{ \"LADCode\": null }")]
        public async Task ProcessAddNewAuthorityForInvalidContentThrows(string theTouchpoint, string theContent)
        {
            // arrange
            var sut = MakeSUT();

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.EnterMethod("ProcessAddNewAuthorityFor"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"deserialising the submitted content: '{theContent}'"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information("deserialisation complete..."))
                .Returns(Task.CompletedTask);

            // act / assert
            await Assert.ThrowsAsync<MalformedRequestException>(() => sut.ProcessAddNewAuthorityFor(theTouchpoint, theContent, scope));
        }

        /// <summary>
        /// process add area routing detail using valid content meets verification
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task ProcessAddAreaRoutingDetailUsingValidContentMeetsVerification()
        {
            // arrange
            const string theTouchpoint = "0000000123";
            const string theLADCode = "E090000070";
            var theContent = $"{{\"id\":\"{theLADCode}\" }}";

            var sut = MakeSUT();
            GetMock(sut.Authorities)
                .Setup(x => x.Add(It.IsAny<IncomingLocalAuthority>()))
                .Returns(Task.FromResult<ILocalAuthority>(new LocalAuthority()));
            GetMock(sut.Respond)
                .Setup(x => x.Created())
                .Returns(new HttpResponseMessage());

            var scope = MakeStrictMock<IScopeLoggingContext>();
            GetMock(scope)
                .Setup(x => x.EnterMethod("ProcessAddNewAuthorityFor"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"deserialising the submitted content: '{theContent}'"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information("deserialisation complete..."))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"applying missing touchpoint details: '{theTouchpoint}'"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"adding the admin district candidate: '{theLADCode}'"))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"candidate addition complete..."))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"preparing response..."))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.Information($"preparation complete..."))
                .Returns(Task.CompletedTask);
            GetMock(scope)
                .Setup(x => x.ExitMethod("ProcessAddNewAuthorityFor"))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.ProcessAddNewAuthorityFor(theTouchpoint, theContent, scope);

            // assert
            Assert.IsAssignableFrom<HttpResponseMessage>(result);
            GetMock(sut.Authorities).VerifyAll();
            GetMock(sut.Respond).VerifyAll();
            GetMock(sut.Faults).VerifyAll();
            GetMock(sut.SafeOperations).VerifyAll();
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal LocalAuthorityManagementFunctionAdapter MakeSUT()
        {
            var store = MakeStrictMock<IStoreLocalAuthorities>();
            var helper = MakeStrictMock<IHttpResponseMessageHelper>();
            var faults = MakeStrictMock<IProvideFaultResponses>();
            var safe = MakeStrictMock<IProvideSafeOperations>();

            return MakeSUT(store, helper, faults, safe);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <param name="store">the store</param>
        /// <param name="helper">the (response) helper</param>
        /// <param name="faults">the fault (response provider)</param>
        /// <param name="safe">the safe (operations provider)</param>
        /// <returns>the system under test</returns>
        internal LocalAuthorityManagementFunctionAdapter MakeSUT(
            IStoreLocalAuthorities store,
            IHttpResponseMessageHelper helper,
            IProvideFaultResponses faults,
            IProvideSafeOperations safe) =>
                new LocalAuthorityManagementFunctionAdapter(helper, faults, safe, store);
    }
}
