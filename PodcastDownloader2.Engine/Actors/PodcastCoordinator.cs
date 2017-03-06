using System;
using System.Threading.Tasks;
using Akka.Actor;
using PodcastDownloader2.Engine.Messages;
using System.Collections.Generic;
using PodcastDownloader2.Engine.Model;
using System.Linq;

namespace PodcastDownloader2.Engine.Actors
{
    public class PodcastDispatcher : ReceiveActor, IWithUnboundedStash
    {
        private readonly IActorRef _bus;
        private readonly IStash _stash;

        private Dictionary<string, int> _podcasts;

        public PodcastDispatcher(IActorRef bus)
        {
            _bus = bus;
            _stash = Stash;
            _podcasts = new Dictionary<string, int>();

            Become(Ready);
        }

        public IStash Stash { get; set; }

        public void Ready()
        {
            Receive<GetPodcasts>(message => {
                Become(GatheringInfo);
                HandleGetPodcasts(message);
            });
        }

        public void GatheringInfo()
        {
            Receive<GetPodcasts>(message => { Stash.Stash(); });
            Receive<TotalPodcastEpisodesKnown>(message => {
                HandleTotalEpisodesKnown(message);
            });
            Receive<PodcastEpisodeDownloaded>(message => { Stash.Stash(); });
            Receive<PodcastEpisodeSkipped>(message => { Stash.Stash(); });
        }

        public void Busy()
        {
            Receive<GetPodcasts>(message => { Stash.Stash(); });
            Receive<TotalPodcastEpisodesKnown>(message => HandleTotalEpisodesKnown(message));
            Receive<PodcastEpisodeDownloaded>(message => HandlePodcastEpisodeDownloaded(message));
            Receive<PodcastEpisodeSkipped>(message => HandlePodcastEpisodeSkipped(message));
        }

        private void HandlePodcastEpisodeSkipped(PodcastEpisodeSkipped message)
        {
            _podcasts[message.Episode.Podcast.Url] -= 1;
            if (_podcasts[message.Episode.Podcast.Url] == 0)
            {
                _bus.Tell(new PodcastDone(message.Episode.Podcast));
            }

            if (IsAllDone())
            {
                _bus.Tell("all done");
                Become(Ready);
            }
        }

        private void HandlePodcastEpisodeDownloaded(PodcastEpisodeDownloaded message)
        {
            _podcasts[message.Episode.Podcast.Url] -= 1;
            if (_podcasts[message.Episode.Podcast.Url] == 0)
            {
                _bus.Tell(new PodcastDone(message.Episode.Podcast));
            }

            if (IsAllDone()) {
                _bus.Tell("all done");
                Become(Ready);
            }
        }

        private bool IsAllDone()
        {
            foreach(var key in _podcasts.Keys)
            {
                if (_podcasts[key] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void HandleTotalEpisodesKnown(TotalPodcastEpisodesKnown message)
        {
            _podcasts[message.Podcast.Url] = message.Total;
            if (!_podcasts.Values.Any(x => x == -1))
            {
                Become(Busy);
                Stash.UnstashAll();
            }
        }

        private void HandleGetPodcasts(GetPodcasts message)
        {
            message.Podcasts.ForEach(podcast => _podcasts.Add(podcast.Url, -1));
            message.Podcasts.ForEach(podcast => _bus.Tell(new GetPodcast(podcast)));
        }
    }
}
