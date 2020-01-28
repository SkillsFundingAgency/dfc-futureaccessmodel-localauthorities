using System;
using System.Diagnostics.CodeAnalysis;
#if DEBUG
using System.IO;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Microsoft.Extensions.Configuration;
#endif

namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the application settings provider
    /// this is a shim for environment variables and as such, cannot be tested.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class ApplicationSettingsProvider :
            IProvideApplicationSettings
    {
#if DEBUG
        // on the basis we are running local and debugging...
        // load local settings and check your variables are loaded
        // if this is a production deployment (release), this code will be missing

        static string[] _keys = {
            "DocumentStoreID",
            "RoutingDetailCollectionID",
            "LocalAuthorityCollectionID",
            "DocumentStoreEndpointAddress",
            "DocumentStoreAccountKey"
        };

        public ApplicationSettingsProvider()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "local.settings.json"), true, true)
                .AddEnvironmentVariables()
                .Build();

            _keys.ForEach(x =>
            {
                var candidate = GetVariable(x);
                if (It.IsNull(candidate))
                {
                    candidate = config.GetSection("Values")[x];
                    Console.WriteLine($"candidate value for: {x} => '{candidate}'");

                    Environment.SetEnvironmentVariable(x, candidate);
                    Console.WriteLine($"value applied for: {x} => '{GetVariable(x)}'");

                    return;
                }

                Console.WriteLine($"found value for: {x} => '{candidate}'");
            });
        }
#endif

        /// <summary>
        /// get (the) variable
        /// </summary>
        /// <param name="usingTheValuesKey">using the values key</param>
        /// <returns>the value string</returns>
        public string GetVariable(string usingTheValuesKey) =>
            Environment.GetEnvironmentVariable(usingTheValuesKey);
    }
}
