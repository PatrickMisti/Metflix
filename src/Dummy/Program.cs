using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Dummy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();


            var s = await GetAllFromLink(client, "https://aniworld.to/anime/stream/classroom-of-the-elite");
            await GetLinkInfo(client, s.Seasons[0].Series[0]);
            Console.WriteLine();
        }

        static async Task<XmlDocument> GetXml(HttpClient client, string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);

            Sgml.SgmlReader sgml = new Sgml.SgmlReader()
            {
                DocType = "HTML",
                WhitespaceHandling = WhitespaceHandling.All,
                CaseFolding = Sgml.CaseFolding.ToLower,
                InputStream = new StreamReader(await response.Content.ReadAsStreamAsync())
            };

            XmlDocument doc = new XmlDocument();
            doc.Load(sgml);
            return doc;
        }

        static async Task<List<Season>> GetAllSeason(HttpClient client, string url)
        {
            var xml = await GetXml(client, url);

            const string uri = "https://aniworld.to";
            var seasonList = xml.SelectNodes("//*[@id='stream']//ul");
            var getSeasonList = seasonList?.Item(0)?.SelectNodes("*/a");

            var seasons = new List<Season>();
            foreach (XmlElement s in getSeasonList)
            {
                seasons.Add(new Season()
                {
                    SeasonTitle = s.InnerText,
                    SeasonLink = s.GetAttribute("href")
                });
            }

            foreach (var season in seasons)
            {
                var s = await GetXml(client, uri + season.SeasonLink);
                var l = s.SelectNodes("//*[@id='stream']//ul");
                var sl = l?.Item(1)?.SelectNodes("*/a");
                foreach (XmlElement element in sl)
                {
                    season.Series.Add(new Series()
                    {
                        SeriesLink = element.GetAttribute("href"),
                        Title = element.InnerText
                    });
                }
            }

            return seasons;
        }

        static async Task<SeasonInfo> GetSeasonInfo(HttpClient client, string url)
        {
            var xml = await GetXml(client, url);
            // todo wichtig für title image und co
            var seriesInfo = xml.SelectNodes("//*[@id='series']");

            var image = seriesInfo.Item(0).SelectSingleNode("//*[@class='backdrop']") as XmlElement;

            var title = seriesInfo.Item(0).SelectSingleNode("//*[@class='series-title']/h1/span");

            var des = seriesInfo.Item(0).SelectSingleNode("//*[@class='seri_des']");

            var imageString = image != null ? image.GetAttribute("style") : "";

            var imageFrom = imageString.IndexOf("url(") + 4;
            var imageResult = imageString.Substring(imageFrom);
            var result = imageResult.Substring(0, imageResult.Length - 1);
            

            return new SeasonInfo()
            {
                Title = title.InnerText,
                Description = des.InnerText,
                Image = result
            };
        }

        static async Task<SeasonInfo> GetAllFromLink(HttpClient client, string url)
        {
            var i = CheckIfCaptiortest(await GetXml(client, url));
            if (!i) new SeasonInfo(); 
            var info = await GetSeasonInfo(client, url);
            var movie = await GetAllSeason(client, url);
            info.Seasons = movie;
            return info;
        }

        static async Task GetLinkInfo(HttpClient client, Series info)
        {
            const string uri = "https://aniworld.to";
            string i = uri + info.SeriesLink;
            var xml = await GetXml(client, i);

            var objectBox = xml.SelectSingleNode("//*[@class='hosterSiteVideo']");
            var language = objectBox.SelectNodes("//*[@class='changeLanguageBox']//img");
            //todo to get title and data-lang-key match with provider = data-lang-key
            var key = (language.Item(0) as XmlElement)?.GetAttribute("data-lang-key");
            var lang = (language.Item(0) as XmlElement)?.GetAttribute("title");


            var providers = objectBox.SelectNodes("//*[@class='row']//li");
            //todo to get name of every single href index to zero
            providers.Item(0).SelectSingleNode("//h4");
            var langKey = (providers.Item(0) as XmlElement)?.GetAttribute("data-lang-key");
            var proLangKey = (providers.Item(0) as XmlElement)?.GetAttribute("data-link-target");

            Console.WriteLine();
        }

        static bool CheckIfCaptiortest(XmlDocument doc)
        {
            var xml = doc.SelectSingleNode("//*[@class='securityToken']");

            if (xml != null)
            {
                return true;
            }

            return false;
        }
    }
    
}
