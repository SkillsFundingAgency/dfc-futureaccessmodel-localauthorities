namespace DFC.FutureAccessModel.LocalAuthorities.Models
{
    /// <summary>
    /// regular expressions used during record validation
    /// </summary>
    public static class ValidationExpressions
    {
        /// <summary>
        /// standard text validation expression
        /// </summary>
        public const string StandardText = @"^[a-zA-Z ]+((['\,\.\- ][a-zA-Z ])?[a-zA-Z ]*)*$";

        /// <summary>
        /// email address validation expression
        /// </summary>
        public const string EmailAddress = @"^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";

        /// <summary>
        /// phone number validation expression
        /// </summary>
        public const string PhoneNumber = @"^((\\(?0\\d{4}\\)?\\s?\\d{3}\\s?(\\d{3}|\\d{2}))|(\\(?0\\d{3}\\)?\\s?\\d{3}\\s?(\\d{4}|\\d{3}))|(\\(?0\\d{2}\\)?\";
    }
}
