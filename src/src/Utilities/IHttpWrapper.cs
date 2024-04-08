using System.Xml;
using Metflix.Models;

namespace Metflix.Utilities
{
    public interface IHttpWrapper
    {
        Task<XmlDocument> GetXmlDocument(string url);
    }
}
