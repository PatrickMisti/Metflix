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

            Receive<SeriesInfoRequest>(m =>
            {
                try
                {
                    _logger.Debug("Get series from url!");
                    var i = _client.GetDataFromSeriesAsync(m.Url).Result;
                    Sender.Tell(new SeriesInfoResponse(i));
                }
                catch (Exception e)
                {
                    _logger.Error("Could not get Series from url!",e);
                    Sender.Tell(new SeriesInfoResponse(e));
                }
            });

            Receive<StreamMessageRequest>(m =>
            {
                try
                {
                    _logger.Debug("Get streams and language from series!");
                    var i = _client.GetStreamAndLanguageFromSeriesAsync(m.Series).Result;
                    Sender.Tell(new StreamMessageResponse(i.ToList()));
                }
                catch (Exception e)
                {
                    _logger.Error("Could not get Streamlink and language!", e);
                    Sender.Tell(new StreamMessageResponse(e));
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
