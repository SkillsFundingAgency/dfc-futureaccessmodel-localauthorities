namespace DFC.FutureAccessModel.LocalAuthorities.Providers
{
    /// <summary>
    /// i provide application settings
    /// </summary>
    public interface IProvideApplicationSettings
    {
        /// <summary>
        /// get (the) variable
        /// </summary>
        /// <param name="usingTheValuesKey">using the values key</param>
        /// <returns>the value string</returns>
        string GetVariable(string usingTheValuesKey);
    }
}
