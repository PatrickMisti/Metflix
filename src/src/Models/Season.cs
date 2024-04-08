namespace Metflix.Models
{
    public class Season
    {
        public Season() { }
        public Season(string seasonName, string seasonUrl)
        {
            SeasonName = seasonName;
            SeasonUrl = seasonUrl;
        }

        public int SeasonId { get; set; }
        public string SeasonName { get; set; } = null!;
        public string SeasonUrl { get; set; } = null!;
        public int SeasonMetaId { get; set; }

        public List<Series> Series { get; set; } = new();
    }
}
