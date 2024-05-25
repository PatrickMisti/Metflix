using System.Collections.Immutable;
using System.Xml;
using Metflix.Models;
using Metflix.Utilities;
using Serilog;
using Serilog.Core;


namespace Metflix.HttpWrappers
{
    public class AniHttpClient(string? basicUrl = null) : HttpWrapper(basicUrl ?? AniUri)
    {
        private static readonly string AniUri = "https://aniworld.to";
        private readonly string _searchPath = "/ajax/search";
        private readonly Logger _logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{CorrelationId}] {Message}{NewLine}")
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
            var stream = await GetAllAsync(uri);
            var xml = Converts.ConvertHttpToXml(stream);

            var info = xml.SelectNodes("//*[@id='series']")?.Item(0);
            if (info == null) throw new XmlException("Could not find selected xml!");

            (string title,string des, string image) infoRes = info.SearchInfoFromSeries();

            var seasons = xml.GetSeriesNodeFromXml().SearchForAllSeason();

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
                var response = await GetAllAsync(element);
                var rawSubXml = Converts.ConvertHttpToXml(response);
                var allSeries = rawSubXml.GetSeriesNodeFromXml(false).SearchForAllSeries();
                list.AddRange(allSeries.ToList());
            }

            return list.ToImmutableList();
        }

        /// <summary>
        /// Get all information from series with stream link and language of the stream
        /// </summary>
        /// <param name="series">Series</param>
        /// <returns>SeriesInfo</returns>
        public async Task<IList<StreamInfoLinks>> GetStreamAndLanguageFromSeriesAsync(Series series)
        {
            var response = await GetAllAsync(series.Url);
            var xml = Converts.ConvertHttpToXml(response);

            if (xml == null)
            {
                throw new Exception();
            }

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
        public async Task<IList<SearchEntry>> SearchForSeriesAsync(string search)
            => await SearchWithFormDataAsync<SearchEntry>(_searchPath, "keyword", search);
    }
}