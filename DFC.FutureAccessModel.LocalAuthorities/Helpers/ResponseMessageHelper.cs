using System.Net.Http;
using System.Text;
using DFC.FutureAccessModel.LocalAuthorities.Models;
using Newtonsoft.Json;

namespace DFC.FutureAccessModel.LocalAuthorities.Helpers
{
    /// <summary>
    /// the response message helper
    /// </summary>
    public static class ResponseMessageHelper
    {
        /// <summary>
        /// the (required) response content type
        /// </summary>
        public const string ResponseContentType = "application/json";

        /// <summary>
        /// the cosmos db's id tag
        /// </summary>
        public const string CosmosDBIDTag = "\"id\"";

        /// <summary>
        ///  the (desired) proper object tag
        /// </summary>
        public static readonly string ProperObjectTag = $"\"{nameof(ILocalAuthority.LADCode)}\"";

        /// <summary>
        /// set the content of the response
        /// </summary>
        /// <param name="source">the source (response message)</param>
        /// <param name="theContent">the (new) content</param>
        /// <returns>the message with new content</returns>
        public static HttpResponseMessage SetContent<TResource>(this HttpResponseMessage source, TResource theContent)
            where TResource : ILocalAuthority =>
            SetContent(source, JsonConvert.SerializeObject(theContent).Replace(CosmosDBIDTag, ProperObjectTag));

        /// <summary>
        /// set the content of the response
        /// </summary>
        /// <param name="source">the source (response message)</param>
        /// <param name="theContent">the (new) content</param>
        /// <returns>the message with new content</returns>
        public static HttpResponseMessage SetContent(this HttpResponseMessage source, string theContent)
        {
            source.Content = new StringContent(theContent, Encoding.UTF8, ResponseContentType);
            return source;
        }
    }
}
