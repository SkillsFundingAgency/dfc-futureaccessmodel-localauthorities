using DFC.FutureAccessModel.LocalAuthorities.Models;
using System.Net;

namespace DFC.FutureAccessModel.LocalAuthorities.Helpers
{
    /// <summary>
    /// the local http status code helper
    /// </summary>
    public static class LocalHttpStatusCodeHelper
    {
        /// <summary>
        /// as http status code
        /// </summary>
        /// <param name="source">the source  (local http status code)</param>
        /// <returns>cast as a http status code</returns>
        public static HttpStatusCode AsHttpStatusCode(this LocalHttpStatusCode source) =>
            (HttpStatusCode)source;

        /// <summary>
        /// compares to...
        /// </summary>
        /// <param name="source">the source</param>
        /// <param name="candidate">the candidate</param>
        /// <returns>true if they are the same</returns>
        public static bool ComparesTo(this LocalHttpStatusCode source, HttpStatusCode? candidate) =>
            It.Has(candidate) && candidate == (HttpStatusCode)source;
    }
}
