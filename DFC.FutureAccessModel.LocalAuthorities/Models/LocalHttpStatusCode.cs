namespace DFC.FutureAccessModel.LocalAuthorities.Models
{
    /// <summary>
    /// local http status codes
    /// </summary>
    public enum LocalHttpStatusCode
    {
        /// <summary>
        /// an unprocessable entity
        /// </summary>
        UnprocessableEntity = 422,

        /// <summary>
        /// too many requests (returned in a document client exception)
        /// </summary>
        TooManyRequests = 429
    }
}
