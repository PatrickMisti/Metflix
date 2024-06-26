﻿using System.Collections.Immutable;
using Metflix.Controllers.helpers;
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

    public class StreamMessageRequest(Series series) : IStreamMessage
    {
        public Series Series { get; set; } = series;
    }

    public class StreamMessageResponse : IStreamMessage
    {
        public StreamMessageResponse() { }

        public StreamMessageResponse(List<StreamInfoLinks> links)
        {
            Links = links;
        }

        public StreamMessageResponse(Exception exception)
        {
            Success = exception;
        }

        public Exception? Success { get; set; }
        public List<StreamInfoLinks> Links { get; set; } = [];
    }

    public class StreamLinkMessageRequest(ProviderUrl provider) : IStreamMessage
    {
        public ProviderUrl Provider { get; set; } = provider;
    }

    public class StreamLinkMessageResponse : IStreamMessage
    {
        public StreamLinkMessageResponse(string url)
        {
            Url = url;
        }

        public StreamLinkMessageResponse(Exception e)
        {
            Success = e;
        }
        public string Url { get; set; }

        public Exception? Success { get; set; }
    }

}
