using System.Xml;

namespace Metflix.Utilities
{
    public class HttpWrapper(string url) : IHttpWrapper
    {
        protected readonly string _url = url;
        protected readonly HttpClient Client = new();

        public async Task<XmlDocument> GetXmlDocument(string url)
        {
            HttpResponseMessage response = await Client.GetAsync(url);

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
    }
}
