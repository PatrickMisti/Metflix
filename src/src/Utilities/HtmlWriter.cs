using System.Xml;
using Sgml;

namespace Metflix.Utilities
{
    public class HtmlReader: Sgml.SgmlReader
    {
        public HtmlReader(TextReader text)
        {
            InputStream = text;
            DocType = "HTML";
            WhitespaceHandling = WhitespaceHandling.All;
            CaseFolding = CaseFolding.ToLower;
        }

        public HtmlReader(string content) : this(new StreamReader(content))
        {

        }

        public override bool Read()
        {
            bool status = base.Read();
            if (status)
            {
                if (base.NodeType == XmlNodeType.Element)
                {
                    // Got a node with prefix. This must be one
                    // of those "<o:p>" or something else.
                    // Skip this node entirely. We want prefix
                    // less nodes so that the resultant XML 
                    // requires not namespace.
                    if (base.Name.IndexOf(':') > 0)
                        base.Skip();
                }
            }
            return status;
        }

        public XmlDocument ToXmlDocument()
        {
            XmlDocument doc = new();
            doc.Load(this);
            return doc;
        }
    }
}
