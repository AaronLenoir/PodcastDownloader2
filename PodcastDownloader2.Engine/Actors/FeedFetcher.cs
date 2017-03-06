using Akka.Actor;
using PodcastDownloader2.Engine.Messages;
using PodcastDownloader2.Engine.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace PodcastDownloader2.Engine.Actors
{
    public class FeedFetchActor : ReceiveActor
    {
        private readonly IActorRef _bus;

        private readonly Dictionary<Guid, Podcast> _pendingPodcasts;

        public FeedFetchActor(IActorRef bus)
        {
            _bus = bus;
            _pendingPodcasts = new Dictionary<Guid, Podcast>();

            Become(Ready);
        }

        public void Ready()
        {
            Receive<GetPodcast>(message => HandleReceivePodcast(message));
            Receive<DownloadFileCompleted>(message => HandleDownloadFileCompleted(message));
            Receive<DownloadFailed>(message => HandleDownloadFileFailed(message));
        }

        private void HandleDownloadFileCompleted(DownloadFileCompleted message)
        {
            if (_pendingPodcasts.ContainsKey(message.Guid))
            {
                var feedXml = File.ReadAllText(message.Path);
                var podcast = _pendingPodcasts[message.Guid];
                _pendingPodcasts.Remove(message.Guid);
                _bus.Tell(new PodcastFeedReady(podcast, feedXml));
            }
        }

        private void HandleDownloadFileFailed(DownloadFailed message)
        {
            if (_pendingPodcasts.ContainsKey(message.Guid))
            {
                // Report error better?
                var podcast = _pendingPodcasts[message.Guid];
                var errorMessage = $"Download of '{podcast.Name}' feed from URL '{podcast.Url}' failed: '{message.Exception}'.";
                _pendingPodcasts.Remove(message.Guid);
                _bus.Tell(errorMessage);
            }
        }

        private void HandleReceivePodcast(GetPodcast message)
        {
            var guid = Guid.NewGuid();
            _pendingPodcasts.Add(guid, message.Podcast);
            _bus.Tell(new DownloadFile(guid, message.Podcast.Url));
        }
    }
}
