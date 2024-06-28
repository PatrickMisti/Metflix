using Akka.Actor;
using Metflix.Services.Akka;
using Metflix.Services.Bag;
using Metflix.Services.Message;

namespace Metflix.Services;

public class Dispatcher: ReceiveActor, IReceiveActor
{
    private IActorRef? _streamRef;
    public Dispatcher(IServiceProvider? provider)
    { 
        Activate(provider);

        ReceiveAny(m =>
        {
            if (m is IStreamMessage)
            {
                _streamRef.Forward(m);
            }
        });
    }

    public void Activate(IServiceProvider? provider)
    {
        _streamRef = Context.ActorOf(StreamActor.Prop(provider));
    }

    public static Props Prop()
    {
        return Props.Create(() => new Dispatcher(null));
    }

    public static Props Prop(IServiceProvider provider)
    {
        return Props.Create(() => new Dispatcher(provider));
    }

    public static string ActorName => nameof(Dispatcher);
}