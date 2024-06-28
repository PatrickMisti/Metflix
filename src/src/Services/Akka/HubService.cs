using Akka.Actor;
using Akka.DependencyInjection;

namespace Metflix.Services.Akka
{
    public class HubService(IServiceProvider provider, IHostApplicationLifetime application, IConfiguration configuration)
        : IHostedService, IActorBridge
    {
        private const string HubName = "akka-metflix";
        private ActorSystem? _actorSystem;
        private readonly IServiceProvider _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        private readonly IHostApplicationLifetime _applicationLifetime = application ?? throw new ArgumentNullException(nameof(application));
        private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        private IActorRef? _actorRef;
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var bootstrap = BootstrapSetup.Create();

            // enable DI support inside this ActorSystem, if needed
            var diSetup = DependencyResolverSetup.Create(_serviceProvider);

            // merge this setup (and any others) together into ActorSystemSetup
            var actorSystemSetup = bootstrap.And(diSetup);

            // start ActorSystem

            _actorSystem = ActorSystem.Create(HubName, actorSystemSetup);

            _actorRef = _actorSystem.ActorOf(Dispatcher.Prop(_serviceProvider), Dispatcher.ActorName);

            // add a continuation task that will guarantee shutdown of application if ActorSystem terminates
            //await _actorSystem.WhenTerminated.ContinueWith(tr => {
            //   _applicationLifetime.StopApplication();
            //});
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _actorSystem.WhenTerminated.ContinueWith(_ => {
                _applicationLifetime.StopApplication();
            }, cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // strictly speaking this may not be necessary - terminating the ActorSystem would also work
            // but this call guarantees that the shutdown of the cluster is graceful regardless
            await CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }

        public void Tell(object message)
        {
            _actorRef.Tell(message);
        }

        public Task<T> Ask<T>(object message)
        {
            return _actorRef.Ask<T>(message);
        }
    }
}
