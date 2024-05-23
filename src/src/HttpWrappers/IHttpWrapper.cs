using System.Xml;

namespace Metflix.HttpWrappers
{
    public interface IHttpWrapper
    {
        Task<XmlDocument> GetXmlDocument(string uri);

        string CreateUrl(string uri, string merge = "");
    }
}
