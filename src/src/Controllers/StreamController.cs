using Metflix.Models;
using Metflix.Services.Akka;
using Metflix.Services.Message;
using Microsoft.AspNetCore.Mvc;

namespace Metflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController(ILogger<StreamController> logger, IActorBridge bridge) : ControllerBase
    {
        [HttpGet]
        [Route("GetPopularity")]
        public async Task<List<PopularitySeries>> GetPopularity()
        {
            var result = await bridge.Ask<PopularityMessageResponse>(PopularityMessageRequest.Instance);

            if (result.Success != null)
                throw new BadHttpRequestException(result.Success.Message);

            return result.Series.ToList();
        }

        // POST api/<AkkaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            bridge.Tell(value);
        }
    }
}
