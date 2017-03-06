using Akka.Actor;
using Akka.Routing;
using PodcastDownloader2.Engine.Actors;
using PodcastDownloader2.Engine.Actors.GeneralPurpose;
using PodcastDownloader2.Engine.Actors.GeneralPurpose.Messages;
using PodcastDownloader2.Engine.Messages;
using PodcastDownloader2.Engine.Model;
using System;
using System.Collections.Generic;

namespace PodcastDownloader2.Engine
{
    public class PodcastDownloader : IDisposable
    {
        private readonly ActorSystem _system;
        private readonly IActorRef _bus;

        public PodcastDownloader(string targetPath, IBridgeReceiver bridgeReceiver)
        {
            _system = ActorSystem.Create(nameof(PodcastDownloader));
            _bus = CreateBus(_system);
            //_bus.Tell(new Subscribe(CreateLogger(_system)));
            _bus.Tell(new Subscribe(CreateDownloadCoordinator(_system)));
            _bus.Tell(new Subscribe(CreateFeedFetcher(_system)));
            _bus.Tell(new Subscribe(CreateFeedParser(_system)));
            _bus.Tell(new Subscribe(CreateEpisodeFetcher(_system)));
            _bus.Tell(new Subscribe(CreateEpisodeMover(_system, targetPath)));
            _bus.Tell(new Subscribe(CreateEpisodeLog(_system, targetPath)));
            _bus.Tell(new Subscribe(CreateBridge(_system, bridgeReceiver)));
            _bus.Tell(new Subscribe(CreatePodcastDispatcher(_system, bridgeReceiver)));
        }

        public void GetPodcast(string url, string name)
        {
            var podcast = new Podcast(url, name);
            _bus.Tell(new GetPodcast(podcast));
        }

        public void GetPodcasts(List<Podcast> podcasts)
        {
            _bus.Tell(new GetPodcasts(podcasts));
        }

        private IActorRef CreateBus(ActorSystem system)
        {
            var props = Props.Create(() => new BusActor());
            return system.ActorOf(props);
        }

        private IActorRef CreateLogger(ActorSystem system)
        {
            var props = Props.Create(() => new LoggingActor());
            return system.ActorOf(props);
        }

        private IActorRef CreateDownloadCoordinator(ActorSystem system)
        {
            var downloadActorProps = Props.Create(() => new FileDownloadActor());
            var props = Props.Create(() => new DownloadCoordinator(downloadActorProps, _bus));
            return system.ActorOf(props);
        }

        private IActorRef CreateFeedFetcher(ActorSystem system)
        {
            var props = Props.Create(() => new FeedFetchActor(_bus));
            return system.ActorOf(props);
        }

        private IActorRef CreateFeedParser(ActorSystem system)
        {
            var props = Props.Create(() => new FeedParseActor(_bus));
            return system.ActorOf(props);
        }

        private IActorRef CreateEpisodeFetcher(ActorSystem system)
        {
            var props = Props.Create(() => new EpisodeFetcher(_bus));
            return system.ActorOf(props);
        }

        private IActorRef CreateEpisodeMover(ActorSystem system, string targetPath)
        {
            var props = Props.Create(() => new EpisodeMover(_bus, targetPath));
            return system.ActorOf(props);
        }

        private IActorRef CreateEpisodeLog(ActorSystem system, string targetPath)
        {
            var props = Props.Create(() => new EpisodeLog(_bus, targetPath));
            return system.ActorOf(props);
        }

        private IActorRef CreateBridge(ActorSystem system, IBridgeReceiver bridgeReceiver)
        {
            var props = Props.Create(() => new BridgeActor(_bus, bridgeReceiver));
            return system.ActorOf(props);
        }

        private IActorRef CreatePodcastDispatcher(ActorSystem system, IBridgeReceiver bridgeReceiver)
        {
            var props = Props.Create(() => new PodcastDispatcher(_bus));
            return system.ActorOf(props);
        }

        public void Dispose()
        {
            _system.Dispose();
        }
    }
}
