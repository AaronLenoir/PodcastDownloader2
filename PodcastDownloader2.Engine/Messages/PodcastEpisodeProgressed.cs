using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastEpisodeProgressed
    {
        public Episode Episode { get; }
        public double Progress { get; }

        public PodcastEpisodeProgressed(Episode episode, double progress)
        {
            Episode = episode;
            Progress = progress;
        }
    }
}
