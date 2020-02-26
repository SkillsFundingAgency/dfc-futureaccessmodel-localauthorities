namespace DFC.FutureAccessModel.LocalAuthorities.Validation
{
    /// <summary>
    /// regular expressions used during record validation
    /// </summary>
    public static class ValidationExpressions
    {
        /// <summary>
        /// local administrative district code expression
        /// </summary>
        public const string LocalAdminDistrictCode = "^([E|S|W])([0-9]{8})";

        /// <summary>
        /// touchpoint identifier expression
        /// </summary>
        public const string TouchpointID = @"^[0-9]*";

        /// <summary>
        /// town or region validation expression
        /// </summary>
        public const string TownOrRegion = @"^[A-Za-z' \-]*";

        /// <summary>
        /// email address validation expression
        /// </summary>
        public const string EmailAddress = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        /// <summary>
        /// phone number validation expression
        /// </summary>
        public const string PhoneNumber = @"^(((\+44\s?\d{3,4}|\(?0\d{3,4}\)?)\s?\d{3,4}\s?\d{3,4})|((\+44\s?\d{3,4}|\(?0\d{3,4}\)?)\s?\d{3,4}\s?\d{3,4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$";
    }
}
