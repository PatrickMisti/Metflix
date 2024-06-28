using System.Collections.Immutable;
using Akka.Actor;
using Metflix.Controllers.helpers;
using Metflix.HttpWrappers;
using Metflix.Models;
using Metflix.Services.Akka;
using Metflix.Services.Message;
using Metflix.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Core;

namespace Metflix.Services.Bag;

public class StreamActor : ReceiveActor, IReceiveActor
{
    private AniHttpClient? _client;
    private readonly TimeSpan _expirationTime = TimeSpan.FromHours(1);
    private readonly TimeSpan _inactiveTime = TimeSpan.FromMinutes(15);
    private readonly string _popularityKey = "popularity";

    private readonly Logger _logger = new LoggerConfiguration()
        .WriteTo.Console()
        .MinimumLevel.Debug()
        .CreateLogger();

    public StreamActor(IServiceProvider? provider)
    {
        _client = new AniHttpClient(Config.HostLink.AniWorld);
        //if (null == _client) throw new ArgumentNullException(nameof(_client));
        var memory = provider?.GetService<IMemoryCache>();
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _expirationTime,
            SlidingExpiration = _inactiveTime
        };



        ReceiveAsync<PopularityMessageRequest>(async m =>
        {
            try
            {
                _logger.Debug("Catch all Popularity Series!");
                if (memory.TryGetValue(_popularityKey, out IImmutableList<PopularitySeries>? result)) Sender.Tell(new PopularityMessageResponse(result!));
                var i = await _client.GetPopularityTitle();
                memory.Set(_popularityKey, i, cacheOptions);
                Sender.Tell(new PopularityMessageResponse(i));
            }
            catch (Exception e)
            {
                _logger.Error("Couldn't get Popularity Series!",e);
                Sender.Tell(new PopularityMessageResponse(e));
            }
        });

        ReceiveAsync<SeriesInfoRequest>(async m =>
        {
            try
            {
                _logger.Debug("Get series from url!");
                var i = await _client.GetDataFromSeriesAsync(m.Url);
                Sender.Tell(new SeriesInfoResponse(i));
            }
            catch (Exception e)
            {
                _logger.Error("Could not get Series from url!",e);
                Sender.Tell(new SeriesInfoResponse(e));
            }
        });

        ReceiveAsync<StreamMessageRequest>(async m =>
        {
            try
            {
                _logger.Debug("Get streams and language from series!");
                var i = await _client.GetStreamAndLanguageFromSeriesAsync(m.Series);
                Sender.Tell(new StreamMessageResponse(i.ToList()));
            }
            catch (Exception e)
            {
                _logger.Error("Could not get Streamlink and language!", e);
                Sender.Tell(new StreamMessageResponse(e));
            }
        });

        ReceiveAsync<StreamLinkMessageRequest>(async m =>
        {
            try
            {
                ProviderUrl provider = m.Provider;
                string result = string.Empty;
                _logger.Debug("Find master link from", provider.provider);

                result = provider.provider switch
                {
                    SeriesProvider.Voe => await _client.SearchLinkForVoeSiteAsync(provider.url),
                    SeriesProvider.Doodstream => throw new Exception("DoodStream not implemented"),
                    SeriesProvider.Vidoza => throw new Exception("Vidoza not implemented"),
                    SeriesProvider.Streamtape => throw new Exception("StreamTape not implemented"),
                    _ => throw new ArgumentOutOfRangeException()
                };
 
                Sender.Tell(new StreamLinkMessageResponse(result));
            }
            catch (Exception e)
            {
                _logger.Error("Could find link for streaming!",e);
                Sender.Tell(new StreamLinkMessageResponse(e));
            }
        });
    }

    /*protected override void PreStart()
    {
        _client = new AniHttpClient(Config.HostLink.AniWorld);
        base.PreStart();
    }*/

    public static Props Prop()
    {
        return Props.Create(() => new StreamActor(null));
    }

    public static Props Prop(IServiceProvider? provider)
    {
        return Props.Create(() => new StreamActor(provider));
    }

    public static string ActorName => nameof(StreamActor);
}