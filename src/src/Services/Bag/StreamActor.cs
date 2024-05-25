using Akka.Actor;
using Metflix.HttpWrappers;
using Metflix.Services.Akka;
using Metflix.Services.Message;
using Metflix.Utilities;
using Serilog;
using Serilog.Core;

namespace Metflix.Services.Bag
{
    public class StreamActor : ReceiveActor, IReceiveActor
    {
        private AniHttpClient? _client;

        private readonly Logger _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();
        public StreamActor()
        {
            _client = new AniHttpClient(Config.HostLink.AniWorld);
            //if (null == _client) throw new ArgumentNullException(nameof(_client));

            Receive<PopularityMessageRequest>( m =>
            {
                try
                {
                    _logger.Debug("Catch all Popularity Series!");
                    var i = _client.GetPopularityTitle().Result;
                    Sender.Tell(new PopularityMessageResponse(i));
                }
                catch (Exception e)
                {
                    _logger.Error("Couldn't get Popularity Series!",e);
                    Sender.Tell(new PopularityMessageResponse(e));
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
            return Props.Create(() => new StreamActor());
        }

        public static string ActorName => nameof(StreamActor);
    }
}
