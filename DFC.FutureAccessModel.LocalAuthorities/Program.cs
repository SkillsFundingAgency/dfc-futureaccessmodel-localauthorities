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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {        
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
    })
    .ConfigureLogging(logging =>
    {
        // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
        // For more information, see https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#application-insights
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();

host.Run();
