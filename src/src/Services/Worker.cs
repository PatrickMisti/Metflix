using Akka.Actor;
using Akka.Configuration;
using Metflix.HttpWrappers;
using Metflix.Services.Message;
using Config = Metflix.Utilities.Config;

namespace Metflix.Services
{
    public class Worker : ReceiveActor
    {
        private List<int> _dummy = [];
        public Worker()
        {
            _dummy.AddRange([1, 2, 3, 4, 5, 6]);
            var i = new AniHttpClient(Config.HostLink.AniWorld);

            Receive<WorkerMessageRequest>(async mess =>
            {
                try
                {
                    
                    //var s = await i.GetDataFromSeriesAsync("/anime/stream/classroom-of-the-elite");
                    //var info = await i.GetStreamAndLanguageFromSeriesAsync(s.Series[0]);
                    //await i.SearchForAnime("classroom");
                    //var s = await i.SearchForSeriesAsync("classroom");

                    var s = await i.GetPopularityTitle();
                    Console.WriteLine("asdf");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }
    }
}
