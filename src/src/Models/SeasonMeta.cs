namespace Metflix.Models
{
    [Serializable]
    public class SeasonMeta
    {
        public SeasonMeta()
        { }

        public SeasonMeta(SeasonInfo seasonInfo, List<Season> seasons)
        {
            Info = seasonInfo;
            Info.SeasonMetaId = Id;
            Seasons = seasons;
            Seasons.ForEach(item => item.SeasonMetaId = Id);
        }

        public int Id { get; set; } = 0;
        public SeasonInfo Info { get; set; } = null!;
        public List<Season> Seasons{ get; set; } = null!;
    }
}
