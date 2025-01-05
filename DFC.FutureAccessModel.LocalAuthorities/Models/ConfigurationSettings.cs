namespace DFC.FutureAccessModel.LocalAuthorities.Models
{
    public class ConfigurationSettings
    {
        public required string DocumentStoreAccountKey { get; set; }
        public required string DocumentStoreEndpointAddress { get; set; }
        public required string DocumentStoreID { get; set; }
        public required string LocalAuthorityCollectionID { get; set; }
    }
}
