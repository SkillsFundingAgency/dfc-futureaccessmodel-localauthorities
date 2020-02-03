using System.Net.Http;
using Newtonsoft.Json;

namespace DFC.FutureAccessModel.LocalAuthorities.Helpers
{
    /// <summary>
    /// the response message helper
    /// </summary>
    public static class ResponseMessageHelper
    {
        /// <summary>
        /// set the content of the response
        /// </summary>
        /// <param name="source">the source (response message)</param>
        /// <param name="theContent">the (new) content</param>
        /// <returns>the message with new content</returns>
        public static HttpResponseMessage SetContent(this HttpResponseMessage source, object theContent) =>
            SetContent(source, JsonConvert.SerializeObject(theContent));

        /// <summary>
        /// set the content of the response
        /// </summary>
        /// <param name="source">the source (response message)</param>
        /// <param name="theContent">the (new) content</param>
        /// <returns>the message with new content</returns>
        public static HttpResponseMessage SetContent(this HttpResponseMessage source, string theContent)
        {
            source.Content = new StringContent(theContent);
            return source;
        }
    }
}
