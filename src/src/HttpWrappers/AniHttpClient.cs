using System.Collections.Immutable;
using System.Xml;
using Metflix.Models;
using Metflix.Services.Exceptions;
using Metflix.Utilities;
using Serilog;
using Serilog.Core;


namespace Metflix.HttpWrappers;

public class AniHttpClient(string basicUrl) : HttpWrapper(basicUrl)
{
    private readonly string _searchPath = "/ajax/search";
    private readonly string _popularityPath = "/beliebte-animes";

    private int _retryCount = 0;

    private readonly Logger _logger = new LoggerConfiguration()
        .WriteTo.Console()
        .MinimumLevel.Debug()
        .CreateLogger();

    /// <summary>
    /// Get all data from series link
    /// /anime/stream/rinkai for Example
    /// Not needed :
    ///     "https://aniworld.to"
    /// </summary>
    /// <param name="uri"></param>
    /// <returns>SeasonMeta</returns>
    public async Task<SeriesInfo> GetDataFromSeriesAsync(string uri)
    {
        var response = await GetAllStreamAsync(uri);
        var xml = response.ConvertHttpToXml();

        var info = xml.SelectNodes("//*[@id='series']")?.Item(0);
        if (info == null) throw new XmlException("Could not find selected xml!");

        (string? title,string? des, string? image) infoRes = info.GetInfoFromSeries();

        var seasons = xml.PrepareFilterForSearch().SearchSeason();

        _logger.Debug("Collect all Infos for Series!");

        return new SeriesInfo(infoRes.title, 
            infoRes.des,
            await Converts.ConvertImageStringToBlob(Client, infoRes.image), 
            await GetSeriesFromSeasons(seasons));
    }

    private async Task<IImmutableList<Series>> GetSeriesFromSeasons(IEnumerable<string> seasons)
    {
        var list = new List<Series>();
        foreach (var element in seasons)
        {
            var response = await GetAllStreamAsync(element);
            var rawSubXml = response.ConvertHttpToXml();
            var allSeries = rawSubXml.PrepareFilterForSearch(false).SearchEpisodes();
            list.AddRange(allSeries.ToList());
        }

        return list.ToImmutableList();
    }

    /// <summary>
    /// Get all information from series with stream link and language of the stream
    /// </summary>
    /// <param name="series">Series</param>
    /// <returns>SeriesInfo</returns>
    public Task<IList<StreamInfoLinks>> GetStreamAndLanguageFromSeriesAsync(Series series)
        => GetStreamAndLanguageFromUrlAsync(series.Url);
    

    public async Task<IList<StreamInfoLinks>> GetStreamAndLanguageFromUrlAsync(string url)
    {
        var response = await GetAllStreamAsync(url);
        var xml = response.ConvertHttpToXml();

        if (xml == null) throw new Exception();

        _logger.Debug("Collect all Data from Stream!");

        return xml.GetStreamLinkInfoFromXml().ToList();
    }

    /// <summary>
    /// https://aniworld.to/search?q=isekai+to+
    ///
    /// Search for series
    ///
    /// need only string of search "classroom"
    /// </summary>
    /// <param name="search">string</param>
    /// <returns>List of SearchEntry</returns>
    public Task<IList<SearchEntry>> SearchForSeriesAsync(string search)
        => SearchWithFormDataAsync<SearchEntry>(_searchPath, "keyword", search);

    public async Task<IImmutableList<PopularitySeries>> GetPopularityTitle()
    {
        var response = await GetAllStreamAsync(_popularityPath);
        var xml = response.ConvertHttpToXml();

        if (xml == null) throw new XmlException("Could not found popularity page!");

        var result = xml.GetAllPopularityTitles();
        var resultList = new List<PopularitySeries>();

        foreach (var element in result)
        {
            var image = await Converts.ConvertImageStringToBlob(Client, element.imgUrl);
            resultList.Add(new PopularitySeries(
                element.title,
                element.category,
                element.url,
                image));
        }

        return resultList.ToImmutableList();
    }


    public async Task<string> SearchLinkForVoeSiteAsync(string url)
        => await TrimVoeSiteForPlayerUrl(
            await GetAllTextAsync(url));
    

    private async Task<string> TrimVoeSiteForPlayerUrl(string text)
    {
        var cuttingFrom = "prompt(\"Node\", \"";
        if (text == null) throw new StreamLinkNotFoundException("Voe stream could not found!");
        int startIndexFrom = text.LastIndexOf(cuttingFrom, StringComparison.Ordinal) + cuttingFrom.Length;
        string raw = text.Substring(startIndexFrom).Trim();

        int endIndexOf = raw.IndexOf("\"", StringComparison.Ordinal);
        if (endIndexOf == -1) return await RedirectionForVoe(text);
        string link = raw?.Substring(0, endIndexOf);

        _logger.Debug("the link is: " + link);
        return link;
    }

    private async Task<string> RedirectionForVoe(string text)
    {
        if (_retryCount > 0) return String.Empty;
        _retryCount++;
        var href = "window.location.href = '";
        var textRaw = text.Substring(text.IndexOf(href)+ href.Length);
        var redirection = textRaw.Substring(0, textRaw.IndexOf("'"));

        var client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(redirection);
        response.EnsureSuccessStatusCode();
        client.Dispose();
        return await TrimVoeSiteForPlayerUrl(await response.Content.ReadAsStringAsync());
    }

    public void ResetRetry() => _retryCount = 0;

    public async Task<string?> SearchLinkForDoodStreamAsync(string url)
    {
        var result = await GetAllStreamAsync(url);
        var xml = Converts.ConvertHttpToXml(result);

        if (xml == null) throw new StreamLinkNotFoundException("DoodStream link could not found!");

        var video = xml.SelectSingleNode("//*[@id='video_player_html5_api']") as XmlElement;

        var link = video?.GetAttribute("src");

        return link;
    }
}