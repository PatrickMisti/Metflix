using Metflix.Controllers.helpers;
using Metflix.Models;
using Metflix.Services.Akka;
using Metflix.Services.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Metflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController(ILogger<StreamController> logger, IActorBridge bridge, IMemoryCache _memoryCache) : ControllerBase
    {
        private readonly string _popularityKey = "popularity";
        private readonly TimeSpan _expirationTime = TimeSpan.FromHours(1);
        private readonly TimeSpan _inactiveTime = TimeSpan.FromMinutes(15);

        [HttpGet]
        [Route("GetPopularity")]
        public async Task<List<PopularitySeries>> GetPopularity()
        {
            if (!_memoryCache.TryGetValue(_popularityKey, out PopularityMessageResponse? result))
            {
                result = await bridge.Ask<PopularityMessageResponse>(PopularityMessageRequest.Instance);

                if (result.Success != null)
                    throw new BadHttpRequestException(result.Success.Message);

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _expirationTime,
                    SlidingExpiration = _inactiveTime
                };

                _memoryCache.Set(_popularityKey, result, cacheOptions);
            }

            return result!.Series.ToList();
        }

        [HttpPost]
        [Route("Series")]   
        public async Task<SeriesInfo> GetSeriesInfoFromUrl(SeriesUrl data)
        {
            var result = await bridge.Ask<SeriesInfoResponse>(new SeriesInfoRequest(data.url));

            if (result.Success != null)
                throw new BadHttpRequestException(result.Success.Message);


            return result.Info!;
        }

        [HttpPost]
        [Route("StreamLink")]
        public async Task<List<StreamInfoLinks>> SendStreamLinks(Series data)
        {
            var result = await bridge.Ask<StreamMessageResponse>(new StreamMessageRequest(data));

            if (result.Success != null)
                throw new BadHttpRequestException(result.Success.Message);


            return result.Links!;
        }

        [HttpPost]
        [Route("Link")]
        public async Task<string> FindM3U8File(ProviderUrlDao data)
        {
            var result = await bridge.Ask<StreamLinkMessageResponse>(new StreamLinkMessageRequest(ProviderUrl.ToMap(data)));

            if (result.Success != null)
                throw new BadHttpRequestException(result.Success.Message);

            return result.Url;
        }
    }
}
