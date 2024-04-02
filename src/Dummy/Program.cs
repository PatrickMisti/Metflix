using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Dummy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://aniworld.to/anime/stream/i-was-reincarnated-as-the-7th-prince-so-i-can-take-my-time-perfecting-my-magical-ability");
            //string i = await response.Content.ReadAsStringAsync();
            //var xml = new XmlDocument();
            //xml.LoadXml(Encoding.Default.GetString(await response.Content.ReadAsByteArrayAsync()));


            XmlDocument document = new XmlDocument();
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    document.Load(reader);
                }
            }

            Console.WriteLine();
        }
    }
}
