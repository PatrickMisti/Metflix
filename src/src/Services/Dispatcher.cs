using Akka.Actor;
using Akka.Util.Internal;
using Metflix.Services.Akka;
using Metflix.Services.Bag;
using Metflix.Services.Message;

namespace Metflix.Services
{
    public class Dispatcher: ReceiveActor, IReceiveActor
    {
        private IActorRef? _streamRef;
        public Dispatcher()
        { 
            Activate();

            ReceiveAny(m =>
            {
                if (m is IStreamMessage)
                {
                    _streamRef.Forward(m);
                }
            });
        }

        public void Activate()
        {
            _streamRef = Context.ActorOf(StreamActor.Prop());
        }

        public static Props Prop()
        {
            return Props.Create(() => new Dispatcher());
        }

        public static string ActorName => nameof(Dispatcher);
    }
}
