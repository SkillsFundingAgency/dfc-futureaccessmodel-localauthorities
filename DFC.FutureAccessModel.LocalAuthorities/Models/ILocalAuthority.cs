namespace DFC.FutureAccessModel.LocalAuthorities.Models
{
    /// <summary>
    /// the local authority (intermediary)
    /// </summary>
    public interface ILocalAuthority
    {
        /// <summary>
        /// the (authorities) touchpoint
        /// </summary>
        string TouchpointID { get; }

        /// <summary>
        /// the local admin district code
        /// </summary>
        string LADCode { get; }

        /// <summary>
        /// the (authority) name
        /// </summary>
        string Name { get; }
    }
}
