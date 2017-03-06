using Akka.Actor;
using PodcastDownloader2.Engine.Messages;
using PodcastDownloader2.Engine.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodcastDownloader2.Engine.Actors
{
    public class EpisodeFetcher : ReceiveActor
    {
        private readonly IActorRef _bus;

        private readonly Dictionary<Guid, Episode> _pendingEpisodes;

        public EpisodeFetcher(IActorRef bus)
        {
            _bus = bus;
            _pendingEpisodes = new Dictionary<Guid, Episode>();

            Become(Ready);
        }

        public void Ready()
        {
            Receive<PodcastEpisodeFound>(message => HandlePodcastEpisodeFound(message));
            Receive<EpisodeNotDownloaded>(message => HandleEpisodeNotDownloaded(message));
            Receive<EpisodeAlreadyDownloaded>(message => HandleEpisodeAlreadyDownloaded(message));
            Receive<DownloadFileCompleted>(message => HandleDownloadFileCompleted(message));
            Receive<DownloadFailed>(message => HandleDownloadFileFailed(message));
            Receive<DownloadProgressed>(message => HandleDownloadFileProgressed(message));
        }

        private void HandleDownloadFileFailed(DownloadFailed message)
        {
            if (_pendingEpisodes.ContainsKey(message.Guid))
            {
                _pendingEpisodes.Remove(message.Guid);
                _bus.Tell($"Podcast episode failed to download downloaded: '{message.Exception}'.");
            }
        }

        private void HandleDownloadFileCompleted(DownloadFileCompleted message)
        {
            if (_pendingEpisodes.ContainsKey(message.Guid))
            {
                var episode = _pendingEpisodes[message.Guid];
                _pendingEpisodes.Remove(message.Guid);
                _bus.Tell(new PodcastEpisodeDownloaded(message.Path, episode));
            }
        }

        private void HandleDownloadFileProgressed(DownloadProgressed message)
        {
            if (_pendingEpisodes.ContainsKey(message.Guid))
            {
                var episode = _pendingEpisodes[message.Guid];
                _bus.Tell(new PodcastEpisodeProgressed(episode, message.Progress));
            }
        }

        public void HandlePodcastEpisodeFound(PodcastEpisodeFound message)
        {
            _bus.Tell(new CheckEpisodeDownloaded(message.Episode));
        }

        public void HandleEpisodeNotDownloaded(EpisodeNotDownloaded message)
        {
            if (!OlderThan(message.Episode.PublishDate, new TimeSpan(30 * 6, 0, 0, 0)))
            {
                var guid = Guid.NewGuid();
                _pendingEpisodes.Add(guid, message.Episode);
                _bus.Tell(new DownloadFile(guid, message.Episode.MediaUri));
            } else
            {
                _bus.Tell(new PodcastEpisodeSkipped(message.Episode, EpisodeSkippedReason.TooOld));
            }
        }

        private void HandleEpisodeAlreadyDownloaded(EpisodeAlreadyDownloaded message)
        {
            _bus.Tell(new PodcastEpisodeSkipped(message.Episode, EpisodeSkippedReason.AlreadyDownloaded));
        }

        private bool OlderThan(DateTimeOffset time, TimeSpan age)
        {
            return (DateTime.Now.Subtract(time.DateTime).TotalSeconds > age.TotalSeconds);
        }
    }
}
