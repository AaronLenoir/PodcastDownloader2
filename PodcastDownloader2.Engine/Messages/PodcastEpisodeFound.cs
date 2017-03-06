using PodcastDownloader2.Engine.Model;
using System;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastEpisodeFound
    {
        public Podcast Podcast { get; }
        public Episode Episode { get; }
        public PodcastEpisodeFound(Podcast podcast, string id, DateTimeOffset publishDate, string title, Uri mediaUri)
        {
            Podcast = podcast;
            Episode = new Episode(podcast, id, publishDate, title, mediaUri);
        }

        public PodcastEpisodeFound(Episode episode)
        {
            Podcast = episode.Podcast;
            Episode = episode;
        }

        public override string ToString()
        {
            return $"Episode of '{Podcast.Name}' found: '{Episode.Title}' ('{Episode.Id}'): '{Episode.MediaUri}'.";
        }
    }

    public enum EpisodeSkippedReason
    {
        Unknown = 0,
        AlreadyDownloaded,
        TooOld
    }

    public class PodcastEpisodeSkipped
    {
        public Episode Episode { get; }
        public EpisodeSkippedReason Reason { get; }
        public PodcastEpisodeSkipped(Episode episode, EpisodeSkippedReason reason)
        {
            Episode = episode;
            Reason = reason;
        }
    }
}
