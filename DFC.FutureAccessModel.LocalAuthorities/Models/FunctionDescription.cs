namespace DFC.FutureAccessModel.LocalAuthorities.Models
{
    public static class FunctionDescription
    {
        public const string ResourceFound = "The requested resource was found";
        public const string ResourceCreated = "The new resource has been created";
        public const string NoContent = "The requested resource was not found";
        public const string NoParentContent = "The parent of this resource was not found";
        public const string Forbidden = "Insufficient access";
        public const string Unauthorised = "API key is unknown or invalid";
        public const string MalformedRequest = "Request was malformed";
        public const string Conflict = "The resource already exists";
    }
}
