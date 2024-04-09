using System.Xml;

namespace Metflix.Utilities
{
    public class HttpWrapper(string url) : IHttpWrapper
    {
        protected readonly string Url = url;
        protected readonly HttpClient Client = new();

        /// <summary>
        /// Response XmlDocument from WebSite
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>XmlDocument</returns>
        public async Task<XmlDocument> GetXmlDocument(string uri)
        {
            HttpResponseMessage response = await Client.GetAsync(uri);

            Sgml.SgmlReader sgml = new Sgml.SgmlReader()
            {
                DocType = "HTML",
                WhitespaceHandling = WhitespaceHandling.All,
                CaseFolding = Sgml.CaseFolding.ToLower,
                InputStream = new StreamReader(await response.Content.ReadAsStreamAsync())
            };

            XmlDocument doc = new XmlDocument();
            doc.Load(sgml);
            return doc;
        }

        public string CreateUrl(string uri,string merge = "")
        {
            return string.Join(merge, Url, uri);
        }
    }
}
