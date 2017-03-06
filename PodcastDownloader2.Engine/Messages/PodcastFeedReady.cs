using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastFeedReady
    {
        public Podcast Podcast { get; }
        public string FeedXml { get; }
        public PodcastFeedReady(Podcast podcast, string feedXml)
        {
            Podcast = podcast;
            FeedXml = feedXml;
        }
    }
}
