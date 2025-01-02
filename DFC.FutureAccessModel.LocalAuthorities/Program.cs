using DFC.Common.Standard.Logging;
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
using DFC.Swagger.Standard;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DFC.FutureAccessModel.LocalAuthorities
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices(services =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddLogging();
                    services.AddSingleton<ILoggerHelper, LoggerHelper>();
                    services.AddSingleton<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
                    services.AddSingleton<IManageLocalAuthorities, LocalAuthorityManagementFunctionAdapter>();
                    services.AddSingleton<ICreateDocumentClients, DocumentClientFactory>();
                    services.AddSingleton<ICreateLoggingContextScopes, LoggingContextScopeFactory>();
                    services.AddSingleton<ICreateValidationMessageContent, ValidationMessageContentFactory>();
                    services.AddSingleton<IProvideApplicationSettings, ApplicationSettingsProvider>();
                    services.AddSingleton<IProvideFaultResponses, FaultResponseProvider>();
                    services.AddSingleton<IProvideSafeOperations, SafeOperationsProvider>();
                    services.AddSingleton<IProvideStoragePaths, StoragePathProvider>();
                    services.AddSingleton<IStoreLocalAuthorities, LocalAuthorityStore>();
                    services.AddSingleton<IStoreDocuments, DocumentStore>();
                    services.AddSingleton<IValidateLocalAuthorities, LocalAuthorityValidator>();

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
