using System.Net;
using System.Text.Json;
using System.Xml;
using Sgml;

namespace Metflix.HttpWrappers
{
    public class HttpWrapper(string url) : IHttpWrapper
    {
        protected readonly string Url = url;
        protected readonly HttpClient Client = new()
        {
            BaseAddress = new Uri(url)
        };

        /// <summary>
        /// Response XmlDocument from WebSite
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>XmlDocument</returns>

        public async Task<XmlDocument> GetXmlDocument(string uri)
        {
            HttpResponseMessage response = await Client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            return ConvertResponseToXml(await response.Content.ReadAsStreamAsync());
        }

        internal XmlDocument ConvertResponseToXml(Stream byteStream)
        {
            try
            {
                SgmlReader sgml = new()
                {
                    DocType = "HTML",
                    WhitespaceHandling = WhitespaceHandling.All,
                    CaseFolding = CaseFolding.ToLower,
                    InputStream = new StreamReader(byteStream)
                };

                XmlDocument doc = new();
                doc.Load(sgml);
                return doc;
            }
            catch (Exception e)
            {
                throw new XmlException("Could not convert stream to xml doc!", e);
            }
        }

        internal async Task<IList<T>> SearchWithFormDataAsync<T>(string path, string key, string search)
        {
            MultipartFormDataContent ss = new()
            {
                {new StringContent(search), key}
            };
            using var result = await Client.PostAsync(path, ss);
            result.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<IList<T>>(await result.Content.ReadAsStreamAsync()) ?? [];
        }

        public string CreateUrl(string uri, string merge = "")
        {
            return string.Join(merge, Url, uri);
        }

        public static string ConvertSearchToUriEncoding(string search)
            => Uri.EscapeDataString(search);
    }
}
