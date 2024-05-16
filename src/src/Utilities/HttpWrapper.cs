using System.Net;
using System.Xml;

namespace Metflix.Utilities
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
        public async Task<XmlDocument> GetXmlDocumentStream(string uri)
        {
            Client.Timeout = TimeSpan.FromMicroseconds(500);
            HttpResponseMessage response = await Client.GetAsync(uri);
            return ConvertResponseToXml(await response.Content.ReadAsStreamAsync());
        }

        public async Task<XmlDocument> GetXmlDocument(string uri)
        {
            HttpResponseMessage response = await Client.GetAsync(uri);

            return ConvertResponseToXml(await response.Content.ReadAsStreamAsync());
        }

        private XmlDocument ConvertResponseToXml(Stream byteStream)
        {
            Sgml.SgmlReader sgml = new Sgml.SgmlReader()
            {
                DocType = "HTML",
                WhitespaceHandling = WhitespaceHandling.All,
                CaseFolding = Sgml.CaseFolding.ToLower,
                InputStream = new StreamReader(byteStream)
            };

            XmlDocument doc = new XmlDocument();
            doc.Load(sgml);
            return doc;
        }

        public string CreateUrl(string uri,string merge = "")
        {
            return string.Join(merge, Url, uri);
        }

        public static string ConvertSearchToUriEncoding(string search) 
            => Uri.EscapeDataString(search);
    }
}
