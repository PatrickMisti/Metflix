using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dummy
{
    public class Season
    {
        public string SeasonTitle { get; set; }
        public string SeasonLink { get; set; }
        public List<Series> Series { get; set; } = new List<Series>();
    }

    public class Series
    {
        public string Title { get; set; }
        public string SeriesLink { get; set; }
    }

    public class SeasonInfo
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Season> Seasons { get; set; }
    }
}
