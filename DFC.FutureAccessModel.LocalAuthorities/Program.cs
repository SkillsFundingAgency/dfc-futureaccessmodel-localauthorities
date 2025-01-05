using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Adapters.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Factories.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using DFC.FutureAccessModel.LocalAuthorities.Providers.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Storage;
using DFC.FutureAccessModel.LocalAuthorities.Storage.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Validation;
using DFC.FutureAccessModel.LocalAuthorities.Validation.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Wrappers;
using DFC.FutureAccessModel.LocalAuthorities.Wrappers.Internal;
using DFC.Swagger.Standard;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFC.FutureAccessModel.LocalAuthorities
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    services.AddOptions<Models.ConfigurationSettings>()
                        .Bind(configuration);

                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddLogging();
                    services.AddSingleton<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
                    services.AddSingleton<IManageLocalAuthorities, LocalAuthorityManagementFunctionAdapter>();
                    services.AddSingleton<ICreateLoggingContextScopes, LoggingContextScopeFactory>();
                    services.AddSingleton<ICreateValidationMessageContent, ValidationMessageContentFactory>();
                    services.AddSingleton<IProvideApplicationSettings, ApplicationSettingsProvider>();
                    services.AddSingleton<IProvideFaultResponses, FaultResponseProvider>();
                    services.AddSingleton<IProvideSafeOperations, SafeOperationsProvider>();
                    services.AddSingleton<IStoreLocalAuthorities, LocalAuthorityStore>();
                    services.AddSingleton<IValidateLocalAuthorities, LocalAuthorityValidator>();
                    services.AddTransient<IWrapCosmosDbClient, CosmosDbWrapper>();

                    services.AddSingleton(s =>
                    {
                        var settings = s.GetRequiredService<IOptions<Models.ConfigurationSettings>>().Value;
                        var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };

                        return new CosmosClient(settings.DocumentStoreEndpointAddress, settings.DocumentStoreAccountKey, options);
                    });

                    services.Configure<LoggerFilterOptions>(options =>
                    {
                        LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
                            == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                        if (toRemove is not null)
                        {
                            options.Rules.Remove(toRemove);
                        }
                    });
                })
                .Build();

            await host.RunAsync();
        }
    }
}
