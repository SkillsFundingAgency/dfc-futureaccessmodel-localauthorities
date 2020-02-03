using System;
using System.Reflection;
using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Adapters.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Registration.Internal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration
{
    public class ServiceRegistrationProviderFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange
            var sut = ServiceRegistrationProvider.CreateService(GetAssemblyfor<ServiceRegistrationProvider>());

            // act / assert
            Assert.IsAssignableFrom<IRegisterServices>(sut);
        }

        /// <summary>
        /// build with null assembly throws
        /// </summary>
        [Fact]
        public void BuildWithNullAssemblyThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new ServiceRegistrationProvider(null));
        }

        /// <summary>
        /// add scoped meets expectation
        /// </summary>
        [Fact]
        public void AddScopedMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT(GetAssemblyfor<ServiceRegistrationProvider>());

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            var registration = new ExternalRegistrationAttribute(typeof(IManageLocalAuthorities), typeof(LocalAuthorityManagementFunctionAdapter), TypeOfRegistrationScope.Scoped);

            // act
            sut.AddScoped(services, registration);

            // assert
            GetMock(services).VerifyAll();
        }

        /// <summary>
        /// add transient meets expectation
        /// </summary>
        [Fact]
        public void AddTransientMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT(GetAssemblyfor<ServiceRegistrationProvider>());

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            var registration = new ExternalRegistrationAttribute(typeof(IManageLocalAuthorities), typeof(LocalAuthorityManagementFunctionAdapter), TypeOfRegistrationScope.Scoped);

            // act
            sut.AddTransient(services, registration);

            // assert
            GetMock(services).VerifyAll();
        }

        /// <summary>
        /// add singleton meets expectation
        /// </summary>
        [Fact]
        public void AddSingletonMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT(GetAssemblyfor<ServiceRegistrationProvider>());

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            var registration = new ExternalRegistrationAttribute(typeof(IManageLocalAuthorities), typeof(LocalAuthorityManagementFunctionAdapter), TypeOfRegistrationScope.Scoped);

            // act
            sut.AddSingleton(services, registration);

            // assert
            GetMock(services).VerifyAll();
        }

        /// <summary>
        /// compose meets expectation
        /// </summary>
        [Fact]
        public void ComposeMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT(GetAssemblyfor<ServiceRegistrationProvider>());

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            // act
            sut.Compose(services);

            // assert
            GetMock(services).Verify(
                x => x.Add(It.IsAny<ServiceDescriptor>()),
                Times.Exactly(12),
                "check for changes in the 'Registration/AssemblyRegistrations.cs' file");
        }

        /// <summary>
        /// get (the) assembly for...
        /// </summary>
        /// <typeparam name="TTarget">the type of target</typeparam>
        /// <returns>the target assembly</returns>
        internal Assembly GetAssemblyfor<TTarget>() =>
            typeof(TTarget).Assembly;

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <param name="theRegistrant">the registrant assembly</param>
        /// <returns>a system under test</returns>
        internal ServiceRegistrationProvider MakeSUT(Assembly theRegistrant) =>
            new ServiceRegistrationProvider(theRegistrant);
    }
}
