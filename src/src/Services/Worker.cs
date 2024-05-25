using Akka.Actor;
using Metflix.HttpWrappers;
using Metflix.Services.Message;

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
                try
                {
                    var s = await i.GetDataFromSeriesAsync("/anime/stream/classroom-of-the-elite");
                    var info = await i.GetStreamAndLanguageFromSeriesAsync(s.Series[0]);
                    //await i.SearchForAnime("classroom");
                    //var s = await i.SearchForSeriesAsync("classroom");
                    Console.WriteLine("asdf");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //await i.SearchForAnime("classroom++");

                Self.Tell(new WorkerMessage(_dummy));
            });
        }
    }
}
