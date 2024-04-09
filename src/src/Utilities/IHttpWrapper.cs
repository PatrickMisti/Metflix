using System.Xml;

namespace Metflix.Utilities
{
    public interface IHttpWrapper
    {
        Task<XmlDocument> GetXmlDocument(string uri);

        string CreateUrl(string uri, string merge = "");
    }
}
