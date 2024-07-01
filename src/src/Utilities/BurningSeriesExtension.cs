using Metflix.Models;
using System.Collections.Immutable;
using System.Xml;
using Metflix.Services.Exceptions;

namespace Metflix.Utilities;

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

        var rawLink = SearchForStreamLink(rawStreamLinks).ToList();
        return SearchForLang(rawLink, rawLanguageBox).ToImmutableList();
    }

    private static IList<(int key, StreamLink)> SearchForStreamLink(XmlNodeList rawStreamLinks)
    {
        // yield return not working with IEnumerable
        List<(int key, StreamLink)> list = [];
        foreach (XmlElement element in rawStreamLinks)
        {
            var result = element.SelectSingleNode("div/a") as XmlElement;//  //div//*[@class='watchEpisode'] not work
            var streamLink = result?.GetAttribute("href");
            var provider = result?.SelectSingleNode("h4")?.InnerText; // //h4 not work
            // data-lang-key is in the li tag
            var langKeyRaw = element?.GetAttribute("data-lang-key");
            bool isKey = int.TryParse(langKeyRaw, out var languageKey);

            if (streamLink.IsEmpty() || provider.IsEmpty() || !isKey)
                throw new StreamLinkNotFoundException("Could not find a stream to watch!");

            list.Add((languageKey, new (streamLink!, provider!)));
        }

        return list;
    }

    private static bool IsEmpty(this string? data) => string.IsNullOrEmpty(data);

    private static IEnumerable<StreamInfoLinks> SearchForLang(IEnumerable<(int key, StreamLink link)> allStreams, XmlNodeList rawLang)
    {
        foreach (XmlElement element in rawLang)
        {
            // get title and lang key from xml
            string langKeyRaw = element?.GetAttribute("data-lang-key") ?? "Nan";
            if (!int.TryParse(langKeyRaw, out var i)) throw new SeriesInfoNotFoundException("Language key not found!");
            var key = int.Parse(langKeyRaw);

            yield return new(
                element?.GetAttribute("title")!,
                from p in allStreams 
                where p.key == key 
                select p.link
                );
        }
    }

    #endregion

    #region Get data from series link

    public static (string? title, string? description, string? image) GetInfoFromSeries(this XmlNode info)
    {
        // find div with class backdrop
        var rawImage = info.SelectSingleNode("//*[@class='backdrop']") as XmlElement;

        return (
            info.SelectSingleNode("//*[@class='series-title']/h1/span")?.InnerText,
            info.SelectSingleNode("//*[@class='seri_des']")?.InnerText, 
            Converts.GetImageUrl(rawImage?.GetAttribute("style")) ?? ""
            );
    }

    public static IEnumerable<string> SearchSeason(this XmlNodeList element)
    {
        foreach (XmlElement item in element)
            yield return item.GetAttribute("href");
    }

    public static IEnumerable<Series> SearchEpisodes(this XmlNodeList seriesList)
    {
        foreach (XmlElement element in seriesList)
            yield return new (
                element.GetAttribute("data-season-id"),
                element.InnerText, 
                element.GetAttribute("href"));
    }

    public static XmlNodeList PrepareFilterForSearch(this XmlDocument xml, bool searchSeason = true)
    {
        if (xml == null) throw new XmlException("There was no content for search!");

        var result = xml.SelectNodes("//*[@id='stream']//ul")
            ?.Item(searchSeason ? 0 : 1)
            ?.SelectNodes("*/a");

        if (result == null) throw new XmlException("Could not select to host");

        return result;
    }
    #endregion

    #region Popularity Links

    public static IEnumerable<(string? title, string? category, string? url, string? imgUrl)> GetAllPopularityTitles(this XmlDocument xml)
    {
        XmlNodeList? containerSeries = xml.SelectNodes("//*[@class='seriesListContainer row']//a");

        if (containerSeries == null) throw new XmlException("Not Found popularity tree!");

        foreach (XmlElement element in containerSeries)
            yield return (
                element?.SelectSingleNode("*//h3")?.InnerText,
                element?.SelectSingleNode("*//small")?.InnerText,
                element?.GetAttribute("href"),
                (element?.SelectSingleNode("*//img") as XmlElement)?.GetAttribute("src")
            );
    }

    #endregion
}