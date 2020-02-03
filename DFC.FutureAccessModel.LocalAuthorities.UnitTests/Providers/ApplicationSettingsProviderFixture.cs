using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the application settings provider fixture
    /// </summary>
    public sealed class ApplicationSettingsProviderFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IProvideApplicationSettings>(MakeSUT());
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal ApplicationSettingsProvider MakeSUT() =>
            new ApplicationSettingsProvider();
    }
}
