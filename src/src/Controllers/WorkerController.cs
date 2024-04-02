using Metflix.Services.Akka;
using Metflix.Services.Message;
using Microsoft.AspNetCore.Mvc;

namespace Metflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController(ILogger<WorkerController> logger, IActorBridge bridge) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<int>> Get()
        {
            var i = await bridge.Ask<WorkerMessage>(WorkerMessageRequest.Instance);
            return i.Message;
        }

        // POST api/<AkkaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            bridge.Tell(value);
        }
    }
}
