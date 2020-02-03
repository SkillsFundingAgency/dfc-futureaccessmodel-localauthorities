using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace DFC.FutureAccessModel.LocalAuthorities.Helpers
{
    /// <summary>
    /// a document extension helper
    /// </summary>
    public static class DocumentHelper
    {
        /// <summary>
        /// convert to...
        /// </summary>
        /// <typeparam name="TReturn">the return type</typeparam>
        /// <param name="fromDocument">from document</param>
        /// <returns></returns>
        public async static Task<TReturn> ConvertTo<TReturn>(this Document fromDocument)
            where TReturn : class
        {
            using (var ms = new MemoryStream())
            {
                using (var reader = new StreamReader(ms))
                {
                    fromDocument.SaveTo(ms);
                    ms.Position = 0;
                    return JsonConvert.DeserializeObject<TReturn>(await reader.ReadToEndAsync());
                }
            }
        }
    }
}
