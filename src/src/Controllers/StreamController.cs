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
        [Route("GetSeries")]
        public async Task<SeriesInfo> GetSeriesInfoFromUrl(string url)
        {
            var result = await bridge.Ask<SeriesInfoResponse>(new SeriesInfoRequest(url));

            if (result.Success != null)
                throw new BadHttpRequestException(result.Success.Message);


            return result.Info!;
        }

    }
}
