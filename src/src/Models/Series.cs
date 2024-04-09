namespace Metflix.Models
{
    [Serializable]
    public class Series
    {
        public Series() { }

        public Series(string seriesNumber, string seriesUrl)
        {
            SeriesNumber = seriesNumber;
            SeriesUrl = seriesUrl;
        }

        public Series(string seriesNumber, string seriesUrl, int seasonId) : this(seriesNumber, seriesUrl)
        {
            SeasonId = seasonId;
        }

        public string SeriesNumber { get; set; } = null!;
        public string SeriesUrl { get; set; } = null!;

        public int SeasonId { get; set; } = 0;
    }
}
