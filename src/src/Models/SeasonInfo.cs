
namespace Metflix.Models
{
    [Serializable]
    public class SeasonInfo
    {
        public SeasonInfo() { }

        public SeasonInfo(byte[] image, string title, string description)
        {
            Image = image;
            Title = title;
            Description = description;
        }

        public int SeasonInfoId { get; set; }
        public byte[] Image { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description{ get; set; } = null!;
        public int SeasonMetaId { get; set; }
    }
}
