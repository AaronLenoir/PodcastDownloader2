using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class GetPodcast
    {
        public Podcast Podcast { get; }
        public GetPodcast(Podcast podcast)
        {
            Podcast = podcast;
        }
    }
}