using DFC.FutureAccessModel.LocalAuthorities.Storage;

namespace DFC.FutureAccessModel.LocalAuthorities.Models
{
    /// <summary>
    /// the local authority
    /// </summary>
    public sealed class IncomingLocalAuthority :
        LocalAuthority
    {
        /// <summary>
        /// here to ensure cosmos db grouping is singular
        /// </summary>
        [PartitionKey]
        public string PartitionKey => "not_required";
    }
}
