namespace Metflix.Models
{
    [Serializable]
    public class SeriesLanguage
    {
        public SeriesLanguage()
        {
        }

        public SeriesLanguage(string languageCode, int languageKey)
        {
            LanguageCode = languageCode;
            LanguageKey = languageKey;
        }

        public string LanguageCode { get; set; } = string.Empty;
        public int LanguageKey { get; set; }
    }
}
