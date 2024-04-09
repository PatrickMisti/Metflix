namespace Metflix.Models
{
    [Serializable]
    public class SeriesStreamLink
    {
        public SeriesStreamLink() { }

        public SeriesStreamLink(string iframeLink, string linkType, int languageKey)
        {
            IframeLink = iframeLink;
            LinkType = linkType;
            LanguageKey = languageKey;
        }

        public string IframeLink { get; set; }
        public string LinkType { get; set; }
        public int LanguageKey { get; set; }
    }
}
