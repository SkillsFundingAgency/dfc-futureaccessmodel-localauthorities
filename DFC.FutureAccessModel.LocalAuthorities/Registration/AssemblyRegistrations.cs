using DFC.Common.Standard.Logging;
using DFC.FutureAccessModel.LocalAuthorities.Adapters;
using DFC.FutureAccessModel.LocalAuthorities.Adapters.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Factories;
using DFC.FutureAccessModel.LocalAuthorities.Factories.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Providers;
using DFC.FutureAccessModel.LocalAuthorities.Providers.Internal;
using DFC.FutureAccessModel.LocalAuthorities.Registration;
using DFC.FutureAccessModel.LocalAuthorities.Storage;
using DFC.FutureAccessModel.LocalAuthorities.Storage.Internal;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.WebJobs.Hosting;

// the web job extension startup registration
[assembly: WebJobsStartup(typeof(LocalAuthoritiesWebJobsExtensionStartup), "Local Authorities Web Jobs Extension Startup")]

// inherited, package level
[assembly: ExternalRegistration(typeof(ILoggerHelper), typeof(LoggerHelper), TypeOfRegistrationScope.Singleton)]
[assembly: ExternalRegistration(typeof(IHttpResponseMessageHelper), typeof(HttpResponseMessageHelper), TypeOfRegistrationScope.Singleton)]
[assembly: ExternalRegistration(typeof(ISwaggerDocumentGenerator), typeof(SwaggerDocumentGenerator), TypeOfRegistrationScope.Singleton)]

// project level
// adapters
[assembly: InternalRegistration(typeof(IManageLocalAuthorities), typeof(LocalAuthorityManagementFunctionAdapter), TypeOfRegistrationScope.Singleton)]

// factories
[assembly: InternalRegistration(typeof(ICreateDocumentClients), typeof(DocumentClientFactory), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(ICreateLoggingContextScopes), typeof(LoggingContextScopeFactory), TypeOfRegistrationScope.Singleton)]

// providers
[assembly: InternalRegistration(typeof(IProvideApplicationSettings), typeof(ApplicationSettingsProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideFaultResponses), typeof(FaultResponseProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideSafeOperations), typeof(SafeOperationsProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideStoragePaths), typeof(StoragePathProvider), TypeOfRegistrationScope.Singleton)]

// storage
[assembly: InternalRegistration(typeof(IStoreLocalAuthorities), typeof(LocalAuthorityStore), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IStoreDocuments), typeof(DocumentStore), TypeOfRegistrationScope.Singleton)]
