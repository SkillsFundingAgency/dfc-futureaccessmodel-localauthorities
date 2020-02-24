using System.Linq;
using System.Reflection;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration
{
    /// <summary>
    /// web jobs ewxtension startup fixture
    /// </summary>
    public sealed class WebJobsExtensionStartupFixture
    {
        /// <summary>
        /// test (the) registrations
        /// this won't test the web job startup class
        /// but we now have some assurances that the registrations
        /// and the 'exported' classes are in sync
        /// </summary>
        [Fact]
        public void TestRegistrations()
        {
            // arrange
            var assembly = Assembly.GetAssembly(typeof(ISupportServiceRegistration));

            // act
            var types = assembly.GetTypes()
                .Where(x => x.IsClass && typeof(ISupportServiceRegistration).IsAssignableFrom(x));
            var registrations = assembly.GetCustomAttributes<InternalRegistrationAttribute>();

            // assert
            Assert.Equal(types.Count(), registrations.Count());
        }
    }
}
