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
                var s = await i.GetAllFromElementAsync("/anime/stream/classroom-of-the-elite");
                var info = await i.GetStreamAndLanguageFromSeriesAsync(s.Seasons[0].Series[0]);
                Self.Tell(new WorkerMessage(_dummy));
            });
        }
    }
}
