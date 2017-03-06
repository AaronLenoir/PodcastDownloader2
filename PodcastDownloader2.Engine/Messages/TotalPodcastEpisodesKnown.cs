using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class TotalPodcastEpisodesKnown
    {
        public Podcast Podcast { get; }
        public int Total { get; }

        public TotalPodcastEpisodesKnown(Podcast podcast, int total)
        {
            Podcast = podcast;
            Total = total;
        }
    }
}
