using System.Collections.Immutable;
using Metflix.Models;

namespace Metflix.Services.Message
{
    public class PopularityMessageRequest : IStreamMessage
    {
        public static PopularityMessageRequest Instance => new();
    }

    public class PopularityMessageResponse : IStreamMessage
    {
        public PopularityMessageResponse(IImmutableList<PopularitySeries> series)
        {
            Series = series;
        }

        public PopularityMessageResponse(Exception exception)
        {
            Success = exception;
        }

        public IImmutableList<PopularitySeries> Series { get; set; } = [];

        public Exception? Success { get; set; }
    }

    public class SeriesInfoRequest(string url) : IStreamMessage
    {
        public string Url { get; set; } = url;
    }

    public class SeriesInfoResponse : IStreamMessage
    {
        public SeriesInfoResponse(){}

        public SeriesInfoResponse(SeriesInfo info)
        {
            Info = info;
        }

        public SeriesInfoResponse(Exception exception)
        {
            Success = exception;
        }

        public Exception? Success { get; set; }
        public SeriesInfo? Info { get; set; }
    }
}
