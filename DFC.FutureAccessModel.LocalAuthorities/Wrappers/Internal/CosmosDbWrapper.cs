using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFC.FutureAccessModel.LocalAuthorities.Wrappers.Internal
{
    public class CosmosDbWrapper : IWrapCosmosDbClient
    {
        private readonly Container _localAuthorityContainer;
        private readonly ILogger<CosmosDbWrapper> _logger;

        public CosmosDbWrapper(CosmosClient cosmosClient,
            IOptions<ConfigurationSettings> configOptions,
            ILogger<CosmosDbWrapper> logger)
        {
            It.IsNull(cosmosClient)
                .AsGuard<ArgumentNullException>(nameof(cosmosClient));

            It.IsNull(configOptions)
                .AsGuard<ArgumentNullException>(nameof(configOptions));

            It.IsNull(logger)
                .AsGuard<ArgumentNullException>(nameof(logger));

            var config = configOptions.Value;

            _localAuthorityContainer = GetContainer(cosmosClient, config.DocumentStoreID, config.LocalAuthorityCollectionID);
            _logger = logger;
        }

        private static Container GetContainer(CosmosClient cosmosClient, string databaseId, string collectionId)
            => cosmosClient.GetContainer(databaseId, collectionId);

        public async Task<LocalAuthority> GetLocalAuthorityAsync(string theAdminDistrict, string partitionKey)
        {
            _logger.LogInformation("Retrieving local authority for the district: {TheAdminDistrict}.", theAdminDistrict);

            try
            {
                var response = await _localAuthorityContainer.ReadItemAsync<LocalAuthority>(theAdminDistrict, new PartitionKey(partitionKey));

                _logger.LogInformation("Local authority retrieved successfully for the district: {TheAdminDistrict}.", theAdminDistrict);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, "Local authority not found for the district: {TheAdminDistrict}. Message: {Message}", theAdminDistrict, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving local authority for the district: {TheAdminDistrict}. Message: {Message} Stack Trace: {StackTrace}", theAdminDistrict, ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> LocalAuthorityExistsAsync(string id, string partitionKey)
        {
            return It.Has(await GetLocalAuthorityAsync(id, partitionKey));
        }

        public async Task<ItemResponse<LocalAuthority>> CreateLocalAuthorityAsync(LocalAuthority localAuthority, string partitionKey)
        {
            if (localAuthority == null)
            {
                _logger.LogError("LocalAuthority object is null. Creation aborted.");
                throw new ArgumentNullException(nameof(localAuthority), "Local authority cannot be null.");
            }

            _logger.LogInformation("Creating local authority with touchpoint: {Touchpoint} and ladcode: {LadCode}", localAuthority.TouchpointID, localAuthority.LADCode);

            try
            {
                var response = await _localAuthorityContainer.CreateItemAsync(localAuthority, new PartitionKey(partitionKey));
                _logger.LogInformation("Successfully created local authority with touchpoint: {Touchpoint} and ladcode: {LadCode}", localAuthority.TouchpointID, localAuthority.LADCode);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create local authority with touchpoint: {Touchpoint} and ladcode: {LadCode}", localAuthority.TouchpointID, localAuthority.LADCode);
                return null;
            }
        }

        public async Task<ItemResponse<LocalAuthority>> DeleteLocalAuthorityAsync(string theAdminDistrict, string partitionKey)
        {
            if (string.IsNullOrEmpty(theAdminDistrict))
            {
                _logger.LogError("{TheAdminDistrict} object is either null or empty. Creation aborted.", nameof(theAdminDistrict));
                throw new ArgumentNullException(nameof(theAdminDistrict), "cannot be null or empty.");
            }

            _logger.LogInformation("Deleting local authority for the district: {TheAdminDistrict}", theAdminDistrict);

            try
            {
                var response = await _localAuthorityContainer.DeleteItemAsync<LocalAuthority>(theAdminDistrict, new PartitionKey(partitionKey));
                _logger.LogInformation("Successfully deleted local authority for the district: {TheAdminDistrict}", theAdminDistrict);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete local authority for the district: {TheAdminDistrict}", theAdminDistrict);
                return null;
            }
        }

    }
}