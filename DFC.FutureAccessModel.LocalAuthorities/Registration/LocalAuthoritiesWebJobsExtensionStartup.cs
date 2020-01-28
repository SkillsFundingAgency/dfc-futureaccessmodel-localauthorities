using System.Diagnostics.CodeAnalysis;
using DFC.Functions.DI.Standard;
using DFC.FutureAccessModel.LocalAuthorities.Registration.Internal;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration
{
    /// <summary>
    /// area routing web jobs extension startup
    /// this can't be tested because the service collection utilises routine
    /// grafting that cannot be 'substituted' (static dpeendency)
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class LocalAuthoritiesWebJobsExtensionStartup :
        IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            var provider = ServiceRegistrationProvider.CreateService();

            provider.Compose(builder.Services);
        }
    }
}