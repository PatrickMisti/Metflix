using Sgml;
using System.Xml;

namespace Metflix.Utilities
{
    public static class Converts
    {
        /// <summary>
        /// Get attribute from div
        /// Attribute is style
        /// get link from style: 'background: url('link you need')'
        /// cut out only link
        /// </summary>
        /// <param name="url"></param>
        /// <returns>string</returns>
        public static string? GetImageUrl(string? url)
        {
            // index from where you cut out the string
            const string toCut = "url(";
            var imageFrom = url?.IndexOf(toCut) + toCut.Length;

            if (string.IsNullOrEmpty(url) || imageFrom < 0) return null;
            // remove till url( <- 
            var imageResult = url.Substring((int)imageFrom!);
            // remove last element -> )
            return imageResult.Substring(0, imageResult.Length - 1);
        }

        /// <summary>
        /// Convert image link to byte array for db
        /// </summary>
        /// <param name="http"></param>
        /// <param name="url"></param>
        /// <returns>byte[]</returns>
        public static async Task<byte[]> ConvertImageStringToBlob(HttpClient http, string? url)
        {
            try
            {
                var response = await http.GetAsync(url);
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not convert url to byte[]", e);
            }
        }

        /// <summary>
        /// Get stream from client and convert it to xml doc
        /// </summary>
        /// <param name="byteStream"></param>
        /// <returns></returns>
        /// <exception cref="XmlException"></exception>
        public static XmlDocument ConvertHttpToXml(Stream byteStream)
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
    }
}
