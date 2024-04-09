namespace Metflix.Models
{
    [Serializable]
    public class SeriesInfo
    {
        public SeriesInfo() { }

        public SeriesInfo(List<SeriesLanguage> languages, List<SeriesStreamLink> streamLinks)
        {
            Languages = languages;
            StreamLinks = streamLinks;
        }

        public List<SeriesLanguage> Languages { get; set; } = new();
        public List<SeriesStreamLink> StreamLinks { get; set; } = new();
    }
}
