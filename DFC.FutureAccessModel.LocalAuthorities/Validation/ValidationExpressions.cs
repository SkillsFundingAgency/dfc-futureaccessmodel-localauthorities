namespace DFC.FutureAccessModel.LocalAuthorities.Validation
{
    /// <summary>
    /// regular expressions used during record validation
    /// </summary>
    public static class ValidationExpressions
    {
        /// <summary>
        /// standard text validation expression
        /// </summary>
        public const string TownOrRegion = @"^[A-Za-z \-]*";

        /// <summary>
        /// email address validation expression
        /// </summary>
        public const string EmailAddress = @"^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";

        /// <summary>
        /// phone number validation expression
        /// </summary>
        public const string PhoneNumber = @"^((\\(?0\\d{4}\\)?\\s?\\d{3}\\s?(\\d{3}|\\d{2}))|(\\(?0\\d{3}\\)?\\s?\\d{3}\\s?(\\d{4}|\\d{3}))|(\\(?0\\d{2}\\)?\";

        /// <summary>
        /// local administrative district code expression
        /// </summary>
        public const string LocalAdminDistrictCode = "^([E|S|W])([0-9]{8})";

        /// <summary>
        /// the touchpoint identifier expression
        /// </summary>
        public const string TouchpointID = "^[0-9]{10}";
    }
}
