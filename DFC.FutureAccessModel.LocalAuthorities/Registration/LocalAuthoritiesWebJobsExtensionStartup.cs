using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DFC.Functions.DI.Standard;
using DFC.FutureAccessModel.LocalAuthorities.Registration.Internal;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration
{
    /// <summary>
    /// local authorities web jobs extension startup
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class LocalAuthoritiesWebJobsExtensionStartup :
        IWebJobsStartup
    {
        /// <summary>
        /// configure uses the service registrar to ensure complete service registration. 
        /// i'd like this to be injectable but i don't have control at this level. 
        /// so i have to use a static factory
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            var registrar = ServiceRegistrationProvider.CreateService(Assembly.GetExecutingAssembly());

            registrar.Compose(builder.Services);
        }
    }
}