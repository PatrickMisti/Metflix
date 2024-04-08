using System.Reflection.Metadata;
using System.Xml;
using Metflix.Models;
using Serilog;
using Serilog.Core;


namespace Metflix.Utilities
{
    public class AniHttpClient() : HttpWrapper(aniUri)
    {
        public static readonly string aniUri = "https://aniworld.to";
        private readonly Logger _logger = new LoggerConfiguration().CreateLogger();

        public async Task<SeasonMeta> GetAllFromContainer(string uri)
        {
            string url = string.Join("", aniUri, uri);
            var doc = await GetXmlDocument(url);
            var seasonInfo = await GetSeasonInfoAsync(doc);
            var seasonList = await GetAllSeasons(doc);

            return new SeasonMeta
            {
                Info = seasonInfo,
                Seasons = seasonList
            };
        }

        #region Helper for get Series Infos
        private async Task<SeasonInfo?> GetSeasonInfoAsync(XmlDocument xml)
        {
            try
            {
                XmlNodeList? seriesInfo = xml.SelectNodes("//*[@id='series']");

                if (seriesInfo == null)
                    throw new Exception("Series info not found!");

                _logger.Debug("Could found data!");

                XmlElement? image = seriesInfo.Item(0)?.SelectSingleNode("//*[@class='backdrop']") as XmlElement;
                XmlNode? title = seriesInfo.Item(0)?.SelectSingleNode("//*[@class='series-title']/h1/span");
                XmlNode? des = seriesInfo.Item(0)?.SelectSingleNode("//*[@class='seri_des']");

                string imageResult = image != null ? GetImageUrl(image.GetAttribute("style")) : "";

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
                return null;
            }
        }

        private async Task<List<Season>> GetAllSeasons(XmlDocument document)
        {
            var seasonList = document.SelectNodes("//*[@id='stream']//ul")
                ?.Item(0)
                ?.SelectNodes("*/a");

            var seasons = new List<Season>();
            foreach (XmlElement element in seasonList)
            {
                var season = await GetSeasonAsync(element);
                if (season != null)
                    seasons.Add(season);
            }

            return seasons;
        }

        private async Task<Season?> GetSeasonAsync(XmlElement seasonDocument)
        {
            var season = new Season
            {
                SeasonName = seasonDocument.InnerText,
                SeasonUrl = seasonDocument.GetAttribute("href")
            };

            if (string.IsNullOrEmpty(season.SeasonUrl))
                return season;

            var seriesListUrl = string.Join("", aniUri + season.SeasonUrl);
            var seriesListDoc = await GetXmlDocument(seriesListUrl);
            XmlNodeList? seriesListRef = seriesListDoc.SelectNodes("//*[@id='stream']//ul")
                ?.Item(1)
                ?.SelectNodes("*/a") ?? null;

            if (seriesListRef == null) return season;

            foreach (XmlElement element in seriesListRef)
                season.Series.Add(new Series()
                {
                    SeriesNumber = element.InnerText,
                    SeriesUrl = element.GetAttribute("href")
                });

            return season;
        }

        private string GetImageUrl(string url)
        {
            var imageFrom = url?.IndexOf("url(") + 4;
            if (string.IsNullOrEmpty(url) || imageFrom < 0) return "";
            var imageResult = url.Substring((int)imageFrom!);
            return imageResult.Substring(0, imageResult.Length - 1);
        }

        private async Task<byte[]> ConvertImageStringToBlob(string url)
        {
            var response = await Client.GetAsync(string.Join("", aniUri, url));
            try
            {
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
