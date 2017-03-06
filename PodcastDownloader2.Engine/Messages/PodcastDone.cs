using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastDone
    {
        public Podcast Podcast { get; private set; }

        public PodcastDone(Podcast podcast)
        {
            Podcast = podcast;
        }
    }
}
