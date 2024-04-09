using System.Xml;
using Metflix.Models;
using Serilog;
using Serilog.Core;


namespace Metflix.Utilities
{
    public class AniHttpClient() : HttpWrapper(AniUri)
    {
        public static readonly string AniUri = "https://aniworld.to";
        private readonly Logger _logger = new LoggerConfiguration().CreateLogger();

        /// <summary>
        /// Get all data from series link
        /// /anime/stream/rinkai for Example
        /// Not needed :
        ///     "https://aniworld.to"
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>SeasonMeta</returns>
        public async Task<SeasonMeta> GetAllFromElementAsync(string uri)
        {
            // create url and get Xml
            var doc = await GetXmlDocument(CreateUrl(uri));
            // return SeasonInfo
            var seasonInfo = await GetSeasonInfoAsync(doc);
            // return List of Season
            var seasonList = await GetAllSeasons(doc);

            // todo maybe make own ExceptionHandling
            _logger.Debug("Create new SeasonMeta with info and list");
            return new SeasonMeta
            {
                Info = seasonInfo,
                Seasons = seasonList
            };
        }

        /// <summary>
        /// Get all information from series with stream link and language of the stream
        /// </summary>
        /// <param name="series">Series</param>
        /// <returns>SeriesInfo</returns>
        public async Task<SeriesInfo> GetStreamAndLanguageFromSeriesAsync(Series series)
        {
            var xmlDoc = await GetXmlDocument(CreateUrl(series.SeriesUrl));
            var hostedSiteVideo = xmlDoc.SelectSingleNode("//*[@class='hosterSiteVideo']");

            if (hostedSiteVideo == null)
            {
                // todo maybe make own ExceptionHandling
                _logger.Error("Could not find hosted site info");
                return new SeriesInfo();
            }

            // ask for language and stream list's
            var languageList = GetSeriesLanguageList(hostedSiteVideo);
            var streamList = GetSeriesStreamLinks(hostedSiteVideo);

            _logger.Debug("Create SeriesInfo for response");

            return new SeriesInfo
            {
                Languages = languageList,
                StreamLinks = streamList
            };
        }

        /// <summary>
        /// https://aniworld.to/search?q=isekai+to+
        ///
        /// Search for series
        /// </summary>
        /// <param name="search"></param>
        /// <returns>List of</returns>
        public async Task SearchForAnime(string search)
        {
            string searchUrl = "/search?q=";
            // replace all space with + and all char like + = %2B or other char into Urlencoded // only special symbols
            // https://www.webatic.com/ascii-table
            //string convertSearch = search.Replace()
        }

        #region Helper for get only SeriesLink infos

        /// <summary>
        /// Convert a list of SeriesLanguage's from xml file
        /// </summary>
        /// <param name="languageBox">XmlNode</param>
        /// <returns>List of SeriesLanguage</returns>
        private List<SeriesLanguage> GetSeriesLanguageList(XmlNode languageBox)
        {
            // in img tag are all data you need
            var rawLanguageBox = languageBox.SelectNodes("//*[@class='changeLanguageBox']//img");

            if (rawLanguageBox == null)
            {
                _logger.Error("Could not find languageBox");
                return new List<SeriesLanguage>();
            }
            // <img src="/public/img/japanese-english.svg"
            // alt="Englische Sprache, English, Flagge, Sprache"
            // data-lang-key="2"
            // title="mit Untertitel Englisch">
            var list = new List<SeriesLanguage>();

            foreach (XmlElement element in rawLanguageBox)
            {
                // get title and lang key from xml
                var languageTitle = element?.GetAttribute("title");
                string langKeyRaw = element?.GetAttribute("data-lang-key") ?? "Nan";
                int.TryParse(langKeyRaw, out var languageKey);

                list.Add(new SeriesLanguage
                {
                    LanguageCode = languageTitle ?? "Default",
                    LanguageKey = languageKey
                });
            }

            _logger.Debug("Resolve all languages");

            return list;
        }

        /// <summary>
        /// Convert a list of all series streams from xml file
        /// </summary>
        /// <param name="seriesStreamBox">XmlNode</param>
        /// <returns>List of SeriesStreamLink</returns>
        private List<SeriesStreamLink> GetSeriesStreamLinks(XmlNode seriesStreamBox)
        {
            // all link are in ul>>class=row -> li -> div -> a
            var rawStreamLinks = seriesStreamBox.SelectNodes("//*[@class='row']//li");

            if (rawStreamLinks == null)
            {
                _logger.Error("Could not find stream links");
                return new List<SeriesStreamLink>();
            }

            var list = new List<SeriesStreamLink>();

            foreach (XmlElement element in rawStreamLinks)
            {
                // result step into the element for href and h4 innerText
                var result = (element.SelectSingleNode("//div/*[@class='watchEpisode']") as XmlElement);
                var streamLink = result?.GetAttribute("href");
                var provider = result?.SelectSingleNode("//h4")?.InnerText;
                // data-lang-key is in the li tag
                var langKeyRaw = element?.GetAttribute("data-lang-key");
                int.TryParse(langKeyRaw,out var languageKey);

                // todo create own Exception
                list.Add(new SeriesStreamLink
                {
                    IframeLink = streamLink ?? "Default",
                    LinkType = provider ?? "Default",
                    LanguageKey = languageKey
                });
            }

            _logger.Debug("Resolve all stream links");

            return list;
        }

        #endregion

        #region Helper for get Season-Series metadata's
        /// <summary>
        /// SeasonInfo includes image, title and description
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>SeasonInfo</returns>
        private async Task<SeasonInfo> GetSeasonInfoAsync(XmlDocument xml)
        {
            try
            {
                XmlNodeList? seriesInfo = xml.SelectNodes("//*[@id='series']");

                if (seriesInfo == null)
                    throw new Exception("Series info not found!");

                _logger.Debug("Could found data!");

                // find div with class backdrop
                XmlElement? image = seriesInfo.Item(0)?.SelectSingleNode("//*[@class='backdrop']") as XmlElement;
                // find title in innerText
                XmlNode? title = seriesInfo.Item(0)?.SelectSingleNode("//*[@class='series-title']/h1/span");
                // find description in innerText
                XmlNode? des = seriesInfo.Item(0)?.SelectSingleNode("//*[@class='seri_des']");

                string imageResult = image != null ? GetImageUrl(image.GetAttribute("style")) : "";

                _logger.Debug("Create SeasonInfo and fill it");

                return new SeasonInfo()
                {
                    Title = title?.InnerText ?? "Not Found",
                    Description = des?.InnerText ?? "NotFount",
                    Image = await ConvertImageStringToBlob(imageResult)
                };
            }
            catch (Exception e)
            {
                _logger.Error("Could not get all infos from Season!!", e);
                return new SeasonInfo();
            }
        }

        /// <summary>
        /// Get all information from season and series
        /// </summary>
        /// <param name="document"></param>
        /// <returns>List of Seasons</returns>
        private async Task<List<Season>> GetAllSeasons(XmlDocument document)
        {
            // To get all season links 
            var seasonList = document.SelectNodes("//*[@id='stream']//ul")
                ?.Item(0)
                ?.SelectNodes("*/a");

            var seasons = new List<Season>();

            // check if XmlNodeList is not null
            if (seasonList == null) return seasons;

            foreach (XmlElement element in seasonList)
            {
                // get all series from season
                var season = await GetSeasonAsync(element);
                if (season != null)
                    seasons.Add(season);
            }

            _logger.Debug("Got all season and there series");

            return seasons;
        }

        /// <summary>
        /// Check out season info and respective series list
        /// </summary>
        /// <param name="seasonDocument"></param>
        /// <returns>Season</returns>
        private async Task<Season?> GetSeasonAsync(XmlElement seasonDocument)
        {
            // innerText is Season 1 / Season 2 & co 
            // href is link to current season where you find all series
            var season = new Season
            {
                SeasonName = seasonDocument.InnerText,
                SeasonUrl = seasonDocument.GetAttribute("href")
            };

            if (string.IsNullOrEmpty(season.SeasonUrl))
                return season;

            var seriesListDoc = await GetXmlDocument(CreateUrl(season.SeasonUrl));
            // Get list of links for every single series
            XmlNodeList? seriesListRef = seriesListDoc.SelectNodes("//*[@id='stream']//ul")
                ?.Item(1)
                ?.SelectNodes("*/a") ?? null;

            if (seriesListRef == null) return season;

            // get all series of element
            foreach (XmlElement element in seriesListRef)
                season.Series.Add(new Series()
                {
                    SeriesNumber = element.InnerText,
                    SeriesUrl = element.GetAttribute("href")
                });

            _logger.Debug("Got all info from one season");

            return season;
        }

        /// <summary>
        /// Get attribute from div
        /// Attribute is style
        /// get link from style: 'background: url('link you need')'
        /// cut out only link
        /// </summary>
        /// <param name="url"></param>
        /// <returns>string</returns>
        private string GetImageUrl(string url)
        {
            // index from where you cut out the string
            const string toCut = "url(";
            var imageFrom = url?.IndexOf(toCut) + toCut.Length;

            if (string.IsNullOrEmpty(url) || imageFrom < 0) return "";
            // remove till url( <- 
            var imageResult = url.Substring((int)imageFrom!);
            // remove last element -> )
            _logger.Debug("String is read for downloading image");
            return imageResult.Substring(0, imageResult.Length - 1);
        }

        /// <summary>
        /// Convert image link to byte array for db
        /// </summary>
        /// <param name="url"></param>
        /// <returns>byte[]</returns>
        private async Task<byte[]> ConvertImageStringToBlob(string url)
        {
            var response = await Client.GetAsync(CreateUrl(url));
            try
            {
                _logger.Debug("Convert image to byte array!");
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception e)
            {
                _logger.Error("Could not convert url to byte[]", e);
                return [];
            }
        }
        #endregion
    }
}