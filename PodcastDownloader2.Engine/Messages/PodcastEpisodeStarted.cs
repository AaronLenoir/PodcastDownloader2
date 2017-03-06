using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastEpisodeStarted
    {
        public Episode Episode { get; }

        public PodcastEpisodeStarted(Episode episode)
        {
            Episode = episode;
        }
    }
}
