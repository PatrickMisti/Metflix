using Akka.Actor;

namespace Metflix.Services.Akka
{
    public interface IReceiveActor
    {
        public static abstract Props Prop();

        public static abstract string ActorName { get; }
    }
}
