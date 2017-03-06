using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class CheckEpisodeDownloaded
    {
        public Episode Episode { get; }

        public CheckEpisodeDownloaded(Episode episode)
        {
            Episode = episode;
        }
    }
}
