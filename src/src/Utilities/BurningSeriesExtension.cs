using Metflix.Models;
using System.Collections.Immutable;
using System.Xml;
using Metflix.Services.Exceptions;

namespace Metflix.Utilities
{
    public static class BurningSeriesExtension
    {
        #region Stream Link and Lang

        /// <summary>
        /// Extension for link and lang from xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <exception cref="XmlException"></exception>
        public static IImmutableList<StreamInfoLinks> GetStreamLinkInfoFromXml(this XmlDocument xml)
        {
            var hostedSiteVideo = xml.SelectSingleNode("//*[@class='hosterSiteVideo']");
            var rawLanguageBox = hostedSiteVideo?.SelectNodes("//*[@class='changeLanguageBox']//img");
            var rawStreamLinks = hostedSiteVideo?.SelectNodes("//*[@class='row']//li");

            if (rawLanguageBox == null || rawStreamLinks == null)
                throw new XmlException("Could not extract stream links from xml!");

            var rawLink = SearchForStreamLink(rawStreamLinks);
            return SearchForLang(rawLink, rawLanguageBox).ToImmutableList();
        }

        private static IEnumerable<(int key, StreamLink)> SearchForStreamLink(XmlNodeList rawStreamLinks)
        {
            foreach (XmlElement element in rawStreamLinks)
            {
                var result = element.SelectSingleNode("//div/*[@class='watchEpisode']") as XmlElement;
                var streamLink = result?.GetAttribute("href");
                var provider = result?.SelectSingleNode("//h4")?.InnerText;
                // data-lang-key is in the li tag
                var langKeyRaw = element?.GetAttribute("data-lang-key");
                bool isKey = int.TryParse(langKeyRaw, out var languageKey);

                if (streamLink.IsEmpty() || provider.IsEmpty() || !isKey)
                    throw new StreamLinkNotFoundException("Could not find a stream to watch!");

                yield return (languageKey, new(streamLink, provider));
            }
        }

        private static bool IsEmpty(this string? data) => string.IsNullOrEmpty(data);

        private static IEnumerable<StreamInfoLinks> SearchForLang(IEnumerable<(int key, StreamLink link)> allStreams, XmlNodeList rawLang)
        {
            foreach (XmlElement element in rawLang)
            {
                // get title and lang key from xml
                var languageTitle = element?.GetAttribute("title");
                string langKeyRaw = element?.GetAttribute("data-lang-key") ?? "Nan";
                var isKey = int.TryParse(langKeyRaw, out var languageKey);

                if (!isKey || languageTitle.IsEmpty())
                    throw new StreamLinkNotFoundException("Could not find language title!");

                var key = int.Parse(langKeyRaw);

                yield return new(languageTitle!, allStreams.Where(p => p.key == key).Select(s => s.link).ToList());
            }
        }

        #endregion

        #region Get data from series link

        public static (string title, string description, string image) SearchInfoFromSeries(this XmlNode info)
        {
            // find div with class backdrop
            var rawImage = info.SelectSingleNode("//*[@class='backdrop']") as XmlElement;

            string? imageResult = Converts.GetImageUrl(rawImage?.GetAttribute("style"));
            var title = info.SelectSingleNode("//*[@class='series-title']/h1/span")?.InnerText;
            var des = info.SelectSingleNode("//*[@class='seri_des']")?.InnerText;

            if (title.IsEmpty() || des.IsEmpty())
                throw new SeriesInfoNotFoundException("Could not find title/des/image from xml!");

            return (title!, des!, imageResult ?? "");
        }

        public static IEnumerable<string> SearchForAllSeason(this XmlNodeList element)
        {
            foreach (XmlElement item in element)
                yield return item.GetAttribute("href");
        }

        public static IEnumerable<Series> SearchForAllSeries(this XmlNodeList seriesList)
        {
            foreach (XmlElement element in seriesList)
                yield return new (
                    element.GetAttribute("data-season-id"),
                    element.InnerText, 
                    element.GetAttribute("href"));
        }

        public static XmlNodeList GetSeriesNodeFromXml(this XmlDocument xml, bool searchSeason = true)
        {
            if (xml == null) throw new XmlException("There was no content for search!");

            int nodeSearch = searchSeason ? 0 : 1;
            string nodeStreamSelection = "//*[@id='stream']//ul";
            string nodeStreamSubSelection = "*/a";

            var result = xml.SelectNodes(nodeStreamSelection)
                ?.Item(nodeSearch)
                ?.SelectNodes(nodeStreamSubSelection);

            if (result == null) throw new XmlException("Could not select to host");

            return result;
        }
        #endregion

        #region Popularity Links

        public static IEnumerable<(string title, string category, string url, string imgUrl)> SearchForPopularityTitle(this XmlDocument xml)
        {
            XmlNodeList? containerSeries = xml.SelectNodes("//*[@class='seriesListContainer row']//a");

            if (containerSeries == null) throw new XmlException("Not Found popularity tree!");

            foreach (XmlElement element in containerSeries)
            {
                var link = element?.GetAttribute("href");
                var category = element?.SelectSingleNode("*//small")?.InnerText;
                var title = element?.SelectSingleNode("*//h3")?.InnerText;
                var rawImage = element?.SelectSingleNode("*//img");
                var image = (rawImage as XmlElement)?.GetAttribute("src");

                if (category.IsEmpty() || title.IsEmpty() || image.IsEmpty() || link.IsEmpty())
                    throw new XmlException("Could not find and popularity series!");

                yield return (title!, category!, link!, image!);
            }
        }

        #endregion
    }
}
