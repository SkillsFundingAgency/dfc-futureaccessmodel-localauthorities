using Newtonsoft.Json;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the validation message item
    /// </summary>
    internal sealed class ValidationMessageItem
    {
        /// <summary>
        /// the (message item) 'code'
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// the (message item) 'message'
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
