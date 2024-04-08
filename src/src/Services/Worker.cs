using Akka.Actor;
using Metflix.Services.Message;
using Metflix.Utilities;

namespace Metflix.Services
{
    public class Worker : ReceiveActor
    {
        private List<int> _dummy = [];
        public Worker()
        {
            _dummy.AddRange([1, 2, 3, 4, 5, 6]);
            var i = new AniHttpClient();

            ReceiveAsync<WorkerMessageRequest>(async mess =>
            {
                var s = await i.GetAllFromContainer("/anime/stream/classroom-of-the-elite");
                Self.Tell(new WorkerMessage(_dummy));
            });
        }
    }
}
