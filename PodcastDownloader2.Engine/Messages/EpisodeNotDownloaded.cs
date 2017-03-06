using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class EpisodeNotDownloaded
    {
        public Episode Episode { get; }

        public EpisodeNotDownloaded(Episode episode)
        {
            Episode = episode;
        }
    }

    public class EpisodeAlreadyDownloaded
    {
        public Episode Episode { get; }

        public EpisodeAlreadyDownloaded(Episode episode)
        {
            Episode = episode;
        }
    }
}
