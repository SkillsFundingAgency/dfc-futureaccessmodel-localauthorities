using System;
using DFC.FutureAccessModel.LocalAuthorities.Registration;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers
{
    /// <summary>
    /// i provide storage paths
    /// </summary>
    public interface IProvideStoragePaths :
        ISupportServiceRegistration
    {
        /// <summary>
        /// the routing detail collection (path), used for new documents
        /// </summary>
        Uri RoutingDetailCollection { get; }

        /// <summary>
        /// the local authority collection (path), used for new documents
        /// </summary>
        Uri LocalAuthorityCollection { get; }

        /// <summary>
        /// get (the) routine detail path for
        /// </summary>
        /// <param name="theTouchpointID">the touchpoint id</param>
        /// <returns>the uri for the requested storage path</returns>
        Uri GetRoutingDetailResourcePathFor(string theTouchpointID);

        /// <summary>
        /// get (the) local authority resource path for...
        /// </summary>
        /// <param name="theAdminDistrict">the admin district</param>
        /// <returns>the uri for the requested storage path</returns>
        Uri GetLocalAuthorityResourcePathFor(string theAdminDistrict);
    }
}