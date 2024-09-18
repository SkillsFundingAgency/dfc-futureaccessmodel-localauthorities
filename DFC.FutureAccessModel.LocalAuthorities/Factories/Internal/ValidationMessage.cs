using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Newtonsoft.Json;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// validation messages
    /// </summary>
    internal sealed class ValidationMessage
    {
        /// <summary>
        /// constructor internalised to prevent use outside the factory
        /// </summary>
        internal ValidationMessage() { }

        /// <summary>
        /// the message collection
        /// </summary>
        private readonly ICollection<ValidationMessageItem> _messages = Collection.Empty<ValidationMessageItem>();

        /// <summary>
        /// the message (errors)
        /// </summary>
        [JsonProperty("errors")]
        public IReadOnlyCollection<ValidationMessageItem> Messages =>
            _messages.AsSafeReadOnlyList();

        /// <summary>
        /// to string
        /// </summary>
        /// <returns>a serialised version of <see cref="ValidationMessage"/></returns>
        public override string ToString() =>
            JsonConvert.SerializeObject(this);

        /// <summary>
        /// add..
        /// </summary>
        /// <param name="theCode">the code</param>
        /// <param name="andMessage">and message</param>
        internal void Add(string theCode, string andMessage)
        {
            var message = new ValidationMessageItem { Code = theCode, Message = andMessage };
            _messages.Add(message);
        }
    }
}